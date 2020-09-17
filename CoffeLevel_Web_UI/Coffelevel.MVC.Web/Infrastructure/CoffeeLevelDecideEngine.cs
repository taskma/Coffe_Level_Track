using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
using CoffeLevel.Data.Repositories;
using CoffeLevel.Data.Entities;
using CoffeLevel.Data;
using System.Web.Mvc;
using System.Net.Http;

namespace ChemistDepository.Client.Web.Infrastructure
{
    public interface ICoffeeLevelDecideEngine
    {

        void decideLevel(CoffeeLevelTable currentLevel, CoffeeInfoTable coffeInfo);
        void insertdataLevelTable(short level, decimal temp, int humidity, short ir0, short ir1, short ir2, short ir3, short ir4, out CoffeeLevelTable levelEntity, out CoffeeInfoTable coffeInfo);
        CoffeeLevelTable getReferanceAvarage(out IEnumerable<CoffeeLevelTable> avarageLevelList);
        void changeCoffeeInfo(CoffeeInfoTable coffeInfo, Guid? referenceLevelID, Guid? referenceNextLevelID, Guid? referenceTempID, Int16 totalDesire);
        void addCalcCoffeeTable(bool hasPot, bool isCooking, Int16 level, CoffeeLevelTable levelEntity, Int16 lastLevel,
                                        CoffeeInfoTable coffeInfo, CoffeeLevelTable currentLevel, bool resetReferance, bool isBadAvarage);
        bool isAfterMinutesThanNow(DateTime lastTime, short minutes);
        CoffeeLevelTable getReferanceAvarageByRef(out IEnumerable<CoffeeLevelTable> avarageLevelList, CoffeeLevelTable refLevel);

    }
    public class CoffeeLevelDecideEngine : ICoffeeLevelDecideEngine
    {
        const Int16 cookingMinPercent = 7;
        const Int16 level1MinPercent = 10;
        const Int16 level2MinPercent = 13;
        const Int16 level3MinPercent = 17;
        const Int16 level4MinPercent = 17;
        const Int16 level5MinPercent = 7;
        const Int16 minRunningTemp = 26;
        const Int16 minCookedMinutes = 2;
        const Int16 maxUpLevelMinutes = 2;
        const Int16 minCancelMinutes = 40;
        short[] decreasingLevelPErcentList = new short[5] { 9, 12, 14, 14, 6 }; // level1, 2, 3, 4, 5
        private ICoffeeLevelRepository _coffeeLevelRepository;
        private ICoffeeInfoRepository _coffeeInfoRepository;
        private ICoffeeCalcLevelRepository _coffeeCalcLevelRepository;
        private ICoffeeEmailRepository _coffeeEmailRepository;
        private IUnitOfWork UnitOfWork;
        private IReferanceLevelRepository _referanceLevelRepository;

        public CoffeeLevelDecideEngine(CoffeeLevelRepository coffeeLevelRepository, CoffeeInfoRepository coffeeInfoRepository, CoffeeCalcLevelRepository coffeeCalcLevelRepository
                                        , CoffeeEmailRepository coffeeEmailRepository, ReferanceLevelRepository referanceLevelRepository)
        {
            this.UnitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();
            _coffeeLevelRepository = coffeeLevelRepository;
            _coffeeInfoRepository = coffeeInfoRepository;
            _coffeeCalcLevelRepository = coffeeCalcLevelRepository;
            _coffeeEmailRepository = coffeeEmailRepository;
            _referanceLevelRepository = referanceLevelRepository;

        }

        public void insertdataLevelTable(short level, decimal temp, int humidity, short ir0, short ir1, short ir2, short ir3, short ir4, out CoffeeLevelTable levelEntity, out CoffeeInfoTable coffeInfo)
        {
            levelEntity = new CoffeeLevelTable();
            levelEntity.Id = Guid.NewGuid();
            levelEntity.level = level;
            levelEntity.temperature = temp;
            levelEntity.humidity = humidity;
            levelEntity.time = DateTime.Now;
            levelEntity.ir0 = ir0;
            levelEntity.ir1 = ir1;
            levelEntity.ir2 = ir2;
            levelEntity.ir3 = ir3;
            levelEntity.ir4 = ir4;
            _coffeeLevelRepository.Add(levelEntity);

            coffeInfo = _coffeeInfoRepository.GetInfo();
            if (coffeInfo == null)
            {
                coffeInfo = new CoffeeInfoTable()
                {
                    Id = Guid.NewGuid(),
                    totalDesire = 0,
                    lastDesireTime = DateTime.Now,
                    referenceLevelID = levelEntity.Id,
                    referenceNextLevelID = levelEntity.Id,
                    referenceTempID = levelEntity.Id,
                };
                _coffeeInfoRepository.Add(coffeInfo);
            }
            UnitOfWork.Commit();
        }

        public void decideLevel(CoffeeLevelTable currentLevel, CoffeeInfoTable coffeInfo)
        {
            bool has10Referance = false;
            bool canChangeRereferance = false;
            IEnumerable<CoffeeLevelTable> afterReferanceAvarageLevels = null;
            CoffeeLevelTable avrLevel = null;
            Int16[] avrLevelIrList = null;
            bool[] isIrDecreasedList = new bool[5];
            var currentLevelIrList = getIRlist(currentLevel);
            CoffeeLevelTable newReferanceLevel = null;
            bool isBadAvarages = false;
            getreferances(coffeInfo, ref has10Referance, ref canChangeRereferance, ref afterReferanceAvarageLevels, ref avrLevel, ref avrLevelIrList, isIrDecreasedList, currentLevelIrList, ref newReferanceLevel, ref isBadAvarages);
            var lastCalcLevel = _coffeeCalcLevelRepository.GetTodayLast();
            var lastCalcLevelHasPot = _coffeeCalcLevelRepository.GetTodayLastHasPot();

            //Pot yoksa (önceki durumda da pot yoksa girmez)
            if (!hasPot(currentLevel) && !(lastCalcLevel != null && !lastCalcLevel.hasPot))
            {
                short level;
                bool isCooking;
                createCalccoffeeForNotHasPot(lastCalcLevel, out level, out isCooking);
                addCalcCoffeeTable(false, isCooking, level, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                return;
            }
            //Öncekinde pot yoksa
            if (lastCalcLevel != null && !lastCalcLevel.hasPot)
            {
                //pot yok ise çık
                if (!hasPot(currentLevel)) return;
                //unutma alarm
                if (!lastCalcLevel.isCooking)
                {
                    if (lastCalcLevel.level == 0)
                    {
                        //referanslar sıfırlanıyor
                        changeCoffeeInfo(coffeInfo, currentLevel.Id, coffeInfo.referenceNextLevelID, coffeInfo.referenceTempID, coffeInfo.totalDesire);
                    }
                }
                addCalcCoffeeTable(true, lastCalcLevel.isCooking, lastCalcLevel.level, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                return;
            }

            //Günün ilk işlemi ise
            if (lastCalcLevel == null)
            {
                //referanslar sıfırlanıyor
                changeCoffeeInfo(coffeInfo, currentLevel.Id, coffeInfo.referenceNextLevelID, coffeInfo.referenceTempID, 0);
                //level 0 ekleniyor
                addCalcCoffeeTable(true, false, 0, currentLevel, 1, coffeInfo, currentLevel, false, false);
                return;
            }
            //Referans hesabı için yeterli data yok ise
            if (!has10Referance)
            {
                return;
            }
            if (isAfterMinutesThanNow(coffeInfo.referenceLevel.time, minCancelMinutes))
            {
                //referanslar sıfırlanıyor
                changeCoffeeInfo(coffeInfo, currentLevel.Id, coffeInfo.referenceNextLevelID, coffeInfo.referenceTempID, 0);
                //level 0 ekleniyor
                addCalcCoffeeTable(true, false, 0, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                return;
            }


            //Sonrakiler
            switch (lastCalcLevel.level)
            {
                //level 0
                case 0:
                    //level1 olmuşmu
                    if (hasChangePercentIncreaseLevel(currentLevel.ir4, avrLevel.ir4, level1MinPercent))
                    {
                        //Pişiyor olmadan level 1 olduysa iptal et
                        if (!lastCalcLevel.isCooking)
                        {
                            changeCoffeeInfo(coffeInfo, newReferanceLevel.Id, coffeInfo.referenceNextLevelID, newReferanceLevel.Id, coffeInfo.totalDesire);
                            return;
                        }
                        addCalcCoffeeTable(true, true, 1, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, isBadAvarages);
                        return;
                    }
                    //Pişmiyor
                    if (!lastCalcLevel.isCooking)
                    {
                        //ir4 Değişim varsa
                        if (hasChangePercentIncreaseLevel(currentLevel.ir4, avrLevel.ir4, cookingMinPercent))
                        {
                            //Pişiyor yap
                            addCalcCoffeeTable(true, true, 0, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, isBadAvarages);
                            return;
                        }
                        //ir4 Değişim yoksa referans değiştir
                        if (canChangeRereferance)
                        {
                            changeCoffeeInfo(coffeInfo, newReferanceLevel.Id, coffeInfo.referenceNextLevelID, newReferanceLevel.Id, coffeInfo.totalDesire);
                        }

                        return;
                    }

                    if (isAfterMinutesThanNow(lastCalcLevel.time, 2))
                    {
                        //level 2 olmuş mu kontrol et
                        if (hasChangePercentIncreaseLevel(currentLevel.ir3, avrLevel.ir3, level2MinPercent))
                        {
                            addCalcCoffeeTable(true, true, 1, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                            return;
                        }
                    }
                    //Pişiyorsa zaman aşımını kontrol et
                    if (isAfterMinutesThanNow(lastCalcLevel.time, 3))
                    {
                        //level 0 yap
                        addCalcCoffeeTable(true, false, 0, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, true, false);
                    }

                    break;
                //level 1
                case 1:
                    if (lastCalcLevel.isCooking)
                    {
                        if (hasChangePercentIncreaseLevel(currentLevel.ir3, avrLevel.ir3, level2MinPercent)
                           || isAfterMinutesThanNow(lastCalcLevelHasPot.time, maxUpLevelMinutes))
                        {
                            if (isAfterMinutesThanNow(lastCalcLevelHasPot.time, maxUpLevelMinutes) && !hasChangePercentIncreaseLevel(currentLevel.ir3, avrLevel.ir3, 9))
                            {
                                //level 0 yap
                                addCalcCoffeeTable(true, false, 0, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, true, false);
                                return;
                            }
                            addCalcCoffeeTable(true, true, 2, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                            //toplam istek sifirlaniyor
                            changeCoffeeInfo(coffeInfo, coffeInfo.referenceLevelID, coffeInfo.referenceNextLevelID, coffeInfo.referenceTempID, 0);
                            _coffeeEmailRepository.ResetRecords();
                            UnitOfWork.Commit();

                        }
                        return;
                    }

                    if (isAfterMinutesThanNow(lastCalcLevelHasPot.time, 2))
                    {
                        addCalcCoffeeTable(true, false, 0, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, true, false);
                        return;
                    }
                    //Pişti, eğer değerler düştüyse levelı düşür
                    changeLevelIfDecreased(currentLevel, isIrDecreasedList, coffeInfo, lastCalcLevel);
                    break;
                //level 2
                case 2:
                    if (lastCalcLevel.isCooking)
                    {
                        if (hasChangePercentIncreaseLevel(currentLevel.ir2, avrLevel.ir2, level3MinPercent) ||
                            isAfterMinutesThanNow(lastCalcLevelHasPot.time, maxUpLevelMinutes))
                        {
                            addCalcCoffeeTable(true, true, 3, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                        }
                        return;
                    }
                    //Pişti, eğer değerler düştüyse levelı düşür
                    changeLevelIfDecreased(currentLevel, isIrDecreasedList, coffeInfo, lastCalcLevel);
                    break;
                case 3:
                    if (lastCalcLevel.isCooking)
                    {
                        if (hasChangePercentIncreaseLevel(currentLevel.ir1, avrLevel.ir1, level4MinPercent) )
                          //  isAfterMinutesThanNow(lastCalcLevelHasPot.time, maxUpLevelMinutes))
                        {
                            addCalcCoffeeTable(true, true, 4, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                        }
                        else if (isAfterMinutesThanNow(lastCalcLevelHasPot.time, 3))
                        {
                            //Pişti yap 3 luk kahve için
                            addCalcCoffeeTable(true, false, 5, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                        }
                        return;
                    }
                    //Pişti, eğer değerler düştüyse levelı düşür
                    changeLevelIfDecreased(currentLevel, isIrDecreasedList, coffeInfo, lastCalcLevel);
                    break;
                case 4:
                    //Pişiyor
                    if (lastCalcLevel.isCooking)
                    {
                        if (hasChangePercentIncreaseLevel(currentLevel.ir0, avrLevel.ir0, level5MinPercent) ||
                            isAfterMinutesThanNow(lastCalcLevelHasPot.time, maxUpLevelMinutes))
                        {
                            addCalcCoffeeTable(true, true, 5, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                        }
                        return;
                    }
                    //Pişti, eğer değerler düştüyse levelı düşür
                    changeLevelIfDecreased(currentLevel, isIrDecreasedList, coffeInfo, lastCalcLevel);
                    break;
                case 5:
                    //Pişiyor
                    if (lastCalcLevel.isCooking)
                    {
                        //minumum dakikayı geçtiyse level 5 ve pişti yap
                        if (isAfterMinutesThanNow(lastCalcLevelHasPot.time, minCookedMinutes))
                        {
                            //Pişti yap
                            addCalcCoffeeTable(true, false, 5, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, false, false);
                        }
                        return;
                    }
                    //Pişti, eğer değerler düştüyse levelı düşür
                    changeLevelIfDecreased(currentLevel, isIrDecreasedList, coffeInfo, lastCalcLevel);
                    break;
                default:
                    break;
            }


        }

        private void getreferances(CoffeeInfoTable coffeInfo, ref bool has10Referance, ref bool canChangeRereferance, ref IEnumerable<CoffeeLevelTable> afterReferanceAvarageLevels, ref CoffeeLevelTable avrLevel, ref short[] avrLevelIrList, bool[] isIrDecreasedList, short[] currentLevelIrList, ref CoffeeLevelTable newReferanceLevel, ref bool isBadAvarages)
        {
            var afterReferanceLevels = _coffeeLevelRepository.GetAfterRefenceAll(coffeInfo.referenceLevel.Id, coffeInfo.referenceLevel.time);
            if (afterReferanceLevels.Count() >= 10)
            {
                has10Referance = true;
                afterReferanceAvarageLevels = afterReferanceLevels.Take(10);

                avrLevel = avvaregeCoffeeLevels(afterReferanceAvarageLevels);
                avrLevelIrList = getIRlist(avrLevel);
                createIrDecreaseList(isIrDecreasedList, avrLevelIrList, currentLevelIrList);

                //Kötü avaraj var mı?
                short maxValue = afterReferanceAvarageLevels.Max(m => m.ir4);
                short minValue = afterReferanceAvarageLevels.Min(m => m.ir4);
                var  minchangePercent = changePercentIncreaseLevel(minValue, avrLevel.ir4);
                var maxchangePercent = changePercentIncreaseLevel(maxValue, avrLevel.ir4);


                isBadAvarages = false;
                if ((maxchangePercent - minchangePercent) >= 8)
                {
                    isBadAvarages = true;
                }
            }
            if (afterReferanceLevels.Count() >= 18)
            {
                canChangeRereferance = true;
                newReferanceLevel = afterReferanceLevels.Skip(1).FirstOrDefault();

            }
        }

        public CoffeeLevelTable getReferanceAvarage(out IEnumerable<CoffeeLevelTable> avarageLevelList)
        {
            avarageLevelList = null;
            CoffeeLevelTable result = new CoffeeLevelTable()
            {
                ir0 = 1,
                ir1 = 1,
                ir2 = 1,
                ir3 = 1,
                ir4 = 1
            };
            var info = _coffeeInfoRepository.GetInfo();
            if (info == null) return result;

            var afterReferanceLevels = _coffeeLevelRepository.GetAfterRefenceAll(info.referenceLevel.Id, info.referenceLevel.time);
            if (afterReferanceLevels == null) return result;
            if (afterReferanceLevels.Count() >= 10)
            {
                avarageLevelList = afterReferanceLevels.Take(10);
                return avvaregeCoffeeLevels(avarageLevelList);
            }
            return result;
        }

        public CoffeeLevelTable getReferanceAvarageByRef(out IEnumerable<CoffeeLevelTable> avarageLevelList, CoffeeLevelTable refLevel)
        {
            avarageLevelList = null;
            CoffeeLevelTable result = new CoffeeLevelTable()
            {
                ir0 = 1,
                ir1 = 1,
                ir2 = 1,
                ir3 = 1,
                ir4 = 1
            };

            var afterReferanceLevels = _coffeeLevelRepository.GetAfterRefenceWith40Minutes(refLevel.Id, refLevel.time);
            if (afterReferanceLevels == null) return result;
            if (afterReferanceLevels.Count() >= 10)
            {
                avarageLevelList = afterReferanceLevels.Take(10);
                return avvaregeCoffeeLevels(avarageLevelList);
            }
            return result;
        }

        private void getReferances(CoffeeInfoTable coffeInfo, ref bool has10Referance, ref bool canChangeRereferance)
        {

        }

        public bool isAfterMinutesThanNow(DateTime lastTime, short minutes)
        {
            int afterMinutes = (int)DateTime.Now.Subtract(lastTime).TotalMinutes;
            return afterMinutes >= minutes;
        }

        private void changeLevelIfDecreased(CoffeeLevelTable currentLevel, bool[] isIrDecreasedList, CoffeeInfoTable coffeInfo, CoffeeCalcLevelTable lastCalcLevel)
        {
            short newLevel = controlNewDecreasedLevel(isIrDecreasedList);
            if (newLevel >= 0 && newLevel < lastCalcLevel.level)
            {
                if (lastCalcLevel.level == 5)
                {
                    if (newLevel == 2 && (!isIrDecreasedList[0] && !isIrDecreasedList[1])) return;
                    if (newLevel == 1 && (!isIrDecreasedList[0] && !isIrDecreasedList[1])) return;
                    if (newLevel == 0 && (!isIrDecreasedList[1] && !isIrDecreasedList[3])) return;
                    if (newLevel == 0 && (!isIrDecreasedList[0] && !isIrDecreasedList[1])) return;
                    if (newLevel == 0 && (!isIrDecreasedList[0] && !isIrDecreasedList[3])) return;
                }
                if (lastCalcLevel.level == 4)
                {
                    if (newLevel == 2 && !isIrDecreasedList[1]) return;
                    if (newLevel == 0 && (!isIrDecreasedList[1] && !isIrDecreasedList[3])) return;
                }

                addCalcCoffeeTable(true, false, newLevel, currentLevel, lastCalcLevel.level, coffeInfo, currentLevel, true, false);
            }

            if (newLevel == 0)
            {
                //referanslar sıfırlanıyor
                addReferanceTable((Guid)coffeInfo.referenceLevelID);
                changeCoffeeInfo(coffeInfo, currentLevel.Id, coffeInfo.referenceLevelID, coffeInfo.referenceTempID, coffeInfo.totalDesire);
            }
        }

        private short controlNewDecreasedLevel(bool[] isIrDecreasedList)
        {
            short newLevel = -1;
            for (int i = 0; i <= 4; i++)
            {
                if (isIrDecreasedList[i])
                {
                    newLevel = Convert.ToInt16(irToLevel(i) - 1);
                }
            }

            return newLevel;
        }

        private void createIrDecreaseList(bool[] isIrDecreasedList, short[] avrLevelIrList, short[] currentLevelIrList)
        {
            for (int i = 0; i < 5; i++)
            {
                isIrDecreasedList[i] = hasChangePercentDecreaseLevel(currentLevelIrList[i], avrLevelIrList[i], decreasingLevelPErcentList[i]);
            }
        }

        private Int16 irToLevel(int ir)
        {
            return Convert.ToInt16(((ir - 2) * -1) + 3);
        }

        private static void createCalccoffeeForNotHasPot(CoffeeCalcLevelTable lastCalcLevel, out short level, out bool isCooking)
        {
            level = 0;
            isCooking = false;
            bool isOpen = false;
            // Bugün calc kayıt yok
            if (lastCalcLevel != null)
            {
                level = lastCalcLevel.level;
                isCooking = lastCalcLevel.isCooking;
                isOpen = lastCalcLevel.isOpen;
            }
        }

        public void addCalcCoffeeTable(bool hasPot, bool isCooking, Int16 level, CoffeeLevelTable levelEntity, Int16 lastLevel,
                                        CoffeeInfoTable coffeInfo, CoffeeLevelTable currentLevel, bool resetReferance, bool isBadAvarage)
        {
            CoffeeCalcLevelTable calcLevelEntity = new CoffeeCalcLevelTable()
            {
                Id = Guid.NewGuid(),
                hasPot = hasPot,
                time = DateTime.Now,
                isCooking = isCooking,
                isOpen = levelEntity.temperature >= minRunningTemp,
                level = level,
                isBadAvarage = isBadAvarage
            };
            _coffeeCalcLevelRepository.Add(calcLevelEntity);
            UnitOfWork.Commit();

            if (level == 0 && resetReferance)
            {
                //referanslar sıfırlanıyor
                addReferanceTable((Guid)coffeInfo.referenceLevelID);
                changeCoffeeInfo(coffeInfo, currentLevel.Id, coffeInfo.referenceLevelID, coffeInfo.referenceTempID, coffeInfo.totalDesire);
            }
            if (hasPot && lastLevel != level)
            {
                sendMessageToServer(isCooking, level);
            }

        }

        private void sendMessageToServer(bool isCooking, Int16 level)
        {
            string strUri = "http://kahve.demirkan.com/update.php?level=" + level.ToString() + "&state=" + isCooking.ToString();
            var client = new HttpClient();

            var uri = new Uri(strUri);
            var response = client.GetAsync(uri);
            //  string textResult =  response.ReadAsStringAsync();

        }

        public void addReferanceTable(Guid id)
        {
            ReferanceLevelTable refLevelEntity = new ReferanceLevelTable()
            {
                Id = Guid.NewGuid(),
                time = DateTime.Now,
                levelId = id
            };

            _referanceLevelRepository.Add(refLevelEntity);
            UnitOfWork.Commit();
        }

        private bool hasPot(CoffeeLevelTable levels)
        {
            return !(levels.ir0 < 400 && levels.ir1 < 400 && levels.ir2 < 400 && levels.ir3 < 400 && levels.ir4 < 400);
        }



        public void changeCoffeeInfo(CoffeeInfoTable coffeInfo, Guid? referenceLevelID, Guid? referenceNextLevelID, Guid? referenceTempID, Int16 totalDesire)
        {
            CoffeeInfoTable newInfoTable = new CoffeeInfoTable()
            {
                Id = Guid.NewGuid(),
                lastDesireTime = DateTime.Now,
                referenceLevelID = referenceLevelID,
                referenceNextLevelID = referenceNextLevelID,
                referenceTempID = referenceTempID,
                totalDesire = totalDesire

            };
            // _coffeeInfoRepository.Update(coffeInfo);
            _coffeeInfoRepository.Delete(coffeInfo);
            _coffeeInfoRepository.Add(newInfoTable);
            UnitOfWork.Commit();
        }

        private Int16[] getIRlist(CoffeeLevelTable level)
        {
            Int16[] irList = new Int16[5];
            irList[0] = level.ir0;
            irList[1] = level.ir1;
            irList[2] = level.ir2;
            irList[3] = level.ir3;
            irList[4] = level.ir4;

            return irList;
        }

        public CoffeeLevelTable avvaregeCoffeeLevels(IEnumerable<CoffeeLevelTable> referanceLevels)
        {
            short ir0s = 0, ir1s = 0, ir2s = 0, ir3s = 0, ir4s = 0;
            foreach (var item in referanceLevels)
            {
                ir0s += item.ir0;
                ir1s += item.ir1;
                ir2s += item.ir2;
                ir3s += item.ir3;
                ir4s += item.ir4;
            }

            CoffeeLevelTable result = new CoffeeLevelTable()
            {
                ir0 = avarageNums(ir0s, referanceLevels.Count()),
                ir1 = avarageNums(ir1s, referanceLevels.Count()),
                ir2 = avarageNums(ir2s, referanceLevels.Count()),
                ir3 = avarageNums(ir3s, referanceLevels.Count()),
                ir4 = avarageNums(ir4s, referanceLevels.Count())
            };
            return result;
        }


        private Int16 avarageNums(Int16 nums, decimal count)
        {
            return Convert.ToInt16((nums) / count);
        }

        private bool hasChangePercentIncreaseLevel(Int16 numNew, Int16 numOld, Int16 minPercent)
        {
            decimal result = (numNew - numOld) / Convert.ToDecimal(numOld) * 100;
            return (result <= minPercent * -1);
        }

        private decimal changePercentIncreaseLevel(Int16 numNew, Int16 numOld)
        {
            decimal result = (numNew - numOld) / Convert.ToDecimal(numOld) * 100;
            return result;
        }

        private bool hasChangePercentDecreaseLevel(Int16 numNew, Int16 numOld, short decreasingLevelPercent)
        {
            decimal result = (numNew - numOld) / Convert.ToDecimal(numOld) * 100;
            return (result > (decreasingLevelPercent * -1));
        }

        private void insertLevelTable(short level, decimal temp, int humidity, short ir0, short ir1, short ir2, short ir3, short ir4, out CoffeeLevelTable levelEntity, out CoffeeInfoTable coffeInfo)
        {
            levelEntity = new CoffeeLevelTable();
            levelEntity.Id = Guid.NewGuid();
            levelEntity.level = level;
            levelEntity.temperature = temp;
            levelEntity.humidity = humidity;
            levelEntity.time = DateTime.Now;
            levelEntity.ir0 = ir0;
            levelEntity.ir1 = ir1;
            levelEntity.ir2 = ir2;
            levelEntity.ir3 = ir3;
            levelEntity.ir4 = ir4;
            _coffeeLevelRepository.Add(levelEntity);

            coffeInfo = _coffeeInfoRepository.GetInfo();
            if (coffeInfo == null)
            {
                coffeInfo = new CoffeeInfoTable()
                {
                    Id = Guid.NewGuid(),
                    totalDesire = 0,
                    lastDesireTime = DateTime.Now,
                    referenceLevelID = levelEntity.Id,
                    referenceNextLevelID = levelEntity.Id,
                    referenceTempID = levelEntity.Id,
                };
                _coffeeInfoRepository.Add(coffeInfo);
            }


            UnitOfWork.Commit();
        }


    }


}