using CoffeLevel.Data.Entities;
using CoffeLevel.Client.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using CoffeLevel.Client.Web.Models;
using System.Text;
using CoffeLevel.Data.Repositories;
using CoffeLevel.Client.Web.Controllers;
using ChemistDepository.Client.Web.Infrastructure;

namespace Coffelevel.Client.Web.Controllers
{
    public class ArduinoController : PublicController
    {


        //
        // GET: /Arduino/
        private ICoffeeLevelRepository _coffeeLevelRepository;
        private ICoffeeInfoRepository _coffeeInfoRepository;
        private ICoffeeCalcLevelRepository _coffeeCalcLevelRepository;
        private ICoffeeLevelDecideEngine _coffeeLevelDecideEngine;
        private ICoffeeEmailRepository _coffeeEmailRepository;

        public ArduinoController(CoffeeLevelRepository coffeeLevelRepository, CoffeeInfoRepository coffeeInfoRepository, 
                                 CoffeeCalcLevelRepository coffeeCalcLevelRepository, CoffeeLevelDecideEngine coffeeLevelDecideEngine,  CoffeeEmailRepository coffeeEmailRepository)
        {
            _coffeeLevelRepository = coffeeLevelRepository;
            _coffeeInfoRepository = coffeeInfoRepository;
            _coffeeCalcLevelRepository  = coffeeCalcLevelRepository;
            _coffeeLevelDecideEngine = coffeeLevelDecideEngine;
            _coffeeEmailRepository = coffeeEmailRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Kurulum()
        {
            var levelData = _coffeeLevelRepository.GetLastToday();
            var coffeInfo = _coffeeInfoRepository.GetInfo();

            if (levelData != null)
            {
                _coffeeLevelDecideEngine.changeCoffeeInfo(coffeInfo, levelData.Id, levelData.Id, coffeInfo.referenceTempID, coffeInfo.totalDesire);
            }

            _coffeeLevelDecideEngine.addCalcCoffeeTable(true, false, 0, levelData, 1, coffeInfo, levelData, false, false);
            ResetDesires();

            return View("ShowSucceed", new ErrorModel { ErrorMessage = "Ayarlar sıfırlandı" });
        }

        private void ResetDesires()
        {
            _coffeeEmailRepository.ResetRecords();
            UnitOfWork.Commit();
        }

        public ActionResult ChangeLevel(short level, bool isCooking)
        {
            var levelData = _coffeeLevelRepository.GetLastToday();
            var coffeInfo = _coffeeInfoRepository.GetInfo();
            _coffeeLevelDecideEngine.addCalcCoffeeTable(true, isCooking, level, levelData, levelData.level, coffeInfo, levelData, false, false);
            ResetDesires();
            return View("ShowSucceed", new ErrorModel { ErrorMessage = "Level değişti" });
        }


        public ActionResult InsertCoffee(Int16 level, Decimal temp, Int32 humidity, Int16 ir0, Int16 ir1, Int16 ir2, Int16 ir3, Int16 ir4)
        {
            CoffeeLevelTable levelEntity;
            CoffeeInfoTable coffeInfo;
            //Ölçülen data tabloya kaydediliyor
            _coffeeLevelDecideEngine.insertdataLevelTable(level, temp, humidity, ir0, ir1, ir2, ir3, ir4, out levelEntity, out coffeInfo);
            //Seviye Karar motoru çağırılıyor, 
            _coffeeLevelDecideEngine.decideLevel(levelEntity, coffeInfo);
            var lastCalcLevel = _coffeeCalcLevelRepository.GetTodayLast();

            ArduinoModel model = new ArduinoModel();
            model.levelPercent = _coffeeCalcLevelRepository.GetTodayLast().level;
            short totalDesire = (short) _coffeeEmailRepository.getTotalDesireOfEmailTable();
            model.Message = totalDesire == 0 ? "Kahve istegi yok" :  totalDesire.ToString() + " kahve istegi!";
            string lastLevel = levelMessageProccess(lastCalcLevel);
            model.Message = "s;"+ model.Message + "m;" + lastLevel + "f;";

            return View(model);

        }

        private static string levelMessageProccess(CoffeeCalcLevelTable lastCalcLevel)
        {
            string lastLevel = lastCalcLevel.level.ToString();
            if (lastCalcLevel.level == 5)
            {
                if (lastCalcLevel.isCooking)
                {
                    lastLevel = "4.5/5 Pisiyor...";
                }
                else
                {
                    lastLevel = "5/5 Hazir hemenAl";
                }

            }
            else if (lastCalcLevel.level == 0 )
            {
                if (lastCalcLevel.isCooking)
                {
                    lastLevel = "0/5 Hazirlaniyor";
                }
                else
                {
                    lastLevel = "0/5 Bitti...";
                }
                
            }
            else if (lastCalcLevel.isCooking)
            {
                lastLevel += "/5 Hazirlaniyor";
            }
            else
            {
                lastLevel += "/5 Bitiyor...";
            }

            return lastLevel;
        }


    }
}
