using ChemistDepository.Client.Web.Infrastructure;
using CoffeLevel.Client.Web.Models;
using CoffeLevel.Data.Entities;
using CoffeLevel.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Coffelevel.Client.Web.Controllers
{
    public class CoffeeWebController : Controller
    {
        //
        // GET: /CoffeeWeb/
        private ICoffeeLevelRepository _coffeeLevelRepository;
        private ICoffeeCalcLevelRepository _coffeeCalcLevelRepository;
        private ICoffeeLevelDecideEngine _coffeeLevelDecideEngine;
        private ICoffeeInfoRepository _coffeeInfoRepository;
        private IReferanceLevelRepository _referanceLevelRepository;
        private ICoffeeEmailRepository _coffeeEmailRepository;



        public CoffeeWebController(CoffeeLevelRepository coffeeLevelRepository, CoffeeCalcLevelRepository coffeeCalcLevelRepository,
                                     CoffeeLevelDecideEngine coffeeLevelDecideEngine, CoffeeInfoRepository coffeeInfoRepository,
                                     ReferanceLevelRepository referanceLevelRepository, CoffeeEmailRepository coffeeEmailRepository)
        {
            _coffeeLevelRepository = coffeeLevelRepository;
            _coffeeCalcLevelRepository = coffeeCalcLevelRepository;
            _coffeeLevelDecideEngine = coffeeLevelDecideEngine;
            _coffeeInfoRepository = coffeeInfoRepository;
            _referanceLevelRepository = referanceLevelRepository;
            _coffeeEmailRepository = coffeeEmailRepository;
        }



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TodayLevels()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _coffeeCalcLevelRepository.GetToday().OrderByDescending(o => o.time);
            model.coffeeCalcLevelList = response;
            return View(model);
        }

        public ActionResult AllLevels()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _coffeeCalcLevelRepository.GetQueryable().OrderByDescending(o => o.time);
            model.coffeeCalcLevelList = response;
            return View(model);
        }

        public ActionResult TodayDatas()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _coffeeLevelRepository.GetToday().OrderByDescending(o => o.time);
            model.coffeeLevelList = response;
            return View(model);
        }

        public ActionResult AllReferances()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _referanceLevelRepository.GetQueryable().OrderByDescending(o => o.time);
            model.referanceLevelList = response;
            return View(model);
        }


        public ActionResult TodayDatasMinutes(int last)

        {
            CoffeeWebModel model = new CoffeeWebModel();
            var newAfterTime = DateTime.Now.AddMinutes(last * -1);
            var response = _coffeeLevelRepository.GetQueryable().Where(p => p.time >= newAfterTime).OrderByDescending(o => o.time);
            var lastTime = response.FirstOrDefault().time;
            var firstTime = newAfterTime;

            model.coffeeCalcLevelList = _coffeeCalcLevelRepository.GetQueryable().Where(p => p.time >= firstTime && p.time <= lastTime);


            var info = _coffeeInfoRepository.GetInfo();
            if (info != null)
            {
                IEnumerable<CoffeeLevelTable> avarageLevelList;
                model.referanceTable = _coffeeLevelDecideEngine.getReferanceAvarage(out avarageLevelList);
                model.referanceTable.Id = info.referenceLevel.Id;
                model.avarageLevelList = avarageLevelList;
            }
            model.coffeeLevelList = response;
            return View("TodayDatasPerc", model);
        }

        public ActionResult RefDatas(Guid? inRef)

        {
            CoffeeWebModel model = new CoffeeWebModel();
            CoffeeLevelTable refLevel = null;
            if (inRef == null)
            {
                refLevel = _coffeeInfoRepository.GetInfo().referenceNextLevel;
            }
            else
            {
                refLevel = _coffeeLevelRepository.GetById((Guid)inRef);
            }
            
            if (refLevel == null)
            {
                return View("TodayDatasPerc", model);
            }

            var response = _coffeeLevelRepository.GetAfterRefenceWith40Minutes(refLevel.Id, refLevel.time).OrderByDescending(o => o.time);
            var lastTime = response.FirstOrDefault().time;
            var firstTime = lastTime.AddMinutes(-40);

            model.coffeeCalcLevelList = _coffeeCalcLevelRepository.GetQueryable().Where(p => p.time >= firstTime && p.time <= lastTime);

                IEnumerable<CoffeeLevelTable> avarageLevelList;
                model.referanceTable = _coffeeLevelDecideEngine.getReferanceAvarageByRef(out avarageLevelList, refLevel);
                model.referanceTable.Id = refLevel.Id;
                model.avarageLevelList = avarageLevelList;
            
            model.coffeeLevelList = response.AsQueryable();
            return View("TodayDatasPerc", model);
        }

        public ActionResult TodayDatasPerc()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _coffeeLevelRepository.GetToday().OrderByDescending(o => o.time);
            var info =_coffeeInfoRepository.GetInfo();
            if (info != null)
            {
                IEnumerable<CoffeeLevelTable> avarageLevelList;
              model.referanceTable = _coffeeLevelDecideEngine.getReferanceAvarage(out avarageLevelList);
              model.referanceTable.Id = info.referenceLevel.Id;
              model.avarageLevelList = avarageLevelList;
            }
            model.coffeeLevelList = response;
            return View(model);
        }

        public JsonResult CoffeLevelGetir()
        {

            LevelModel levelModel = new LevelModel();
            var lastCalcLevel = _coffeeCalcLevelRepository.GetTodayLast();
            var coffeeInfo = _coffeeInfoRepository.GetInfo();
            //        levelModel.totalDesire = data.totalDesire;

            var lastLevel = _coffeeLevelRepository.GetLastToday();

            bool noData = false;
            string dataError = "";
            //Arduino dan 10 dakikadır veri gelmiyor ise
            if ((lastLevel != null && _coffeeLevelDecideEngine.isAfterMinutesThanNow(lastLevel.time, 2) && !HttpContext.Request.IsLocal) || lastCalcLevel == null)
            {
                noData = true;
                dataError = "Cihaz Fişi Takılı Değil";
            }
            else if (_coffeeLevelDecideEngine.isAfterMinutesThanNow(coffeeInfo.referenceLevel.time, 40))
            {
                noData = true;
                dataError = "Kahve yok istekte bulunabilirsin";
            }

            if (noData)
            {
                levelModel.coffeeStatus = dataError;
                levelModel.hasPot = true;
                levelModel.isCooking = false;
                levelModel.isOpen = false;
                levelModel.time = DateTime.Now.ToLongTimeString();
                levelModel.level = 0;
                return Json(levelModel, JsonRequestBehavior.AllowGet);
            }

            levelModel.level = lastCalcLevel.isBadAvarage ? (short) 0 : lastCalcLevel.level;
            levelModel.isCooking = lastCalcLevel.isBadAvarage ? false : lastCalcLevel.isCooking;
            levelModel.time = DateTime.Now.ToLongTimeString();
            levelModel.hasPot = lastCalcLevel.hasPot;
            levelModel.isOpen = lastCalcLevel.isOpen;


            if (!levelModel.hasPot)
            {
                levelModel.coffeeStatus = "Pot Yok";
            }
            else
            {

                if (levelModel.isCooking)
                {
                    if (levelModel.level == 0)
                        levelModel.coffeeStatus = "Hazırlanmaya başladı";
                    else
                        levelModel.coffeeStatus = "Hazırlanıyor";
                }
                else
                {
                    if (levelModel.level == 5)
                        levelModel.coffeeStatus = "Kahve hazır";
                    else if (levelModel.level == 0)
                        levelModel.coffeeStatus = "Kahve yok istekte bulunabilirsin";
                    else
                        levelModel.coffeeStatus = "Kahve Bitiyor Koş Koş..";
                }
            }

            string a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            string IP = Request.UserHostName;
            compName = DetermineCompName(IP);

            User.Identity.Name.ToString();

            //      levelModel.totalDesire = _coffeeInfoRepository.GetInfo().totalDesire;
            levelModel.totalDesire = Convert.ToInt16(_coffeeEmailRepository.getTotalDesireOfEmailTable());


            return Json(levelModel, JsonRequestBehavior.AllowGet);
        }

        public string compName = null;
        public ActionResult AllDatas()
        {
            CoffeeWebModel model = new CoffeeWebModel();
            var response = _coffeeLevelRepository.GetQueryable().OrderByDescending(o => o.time);
            model.coffeeLevelList = response;
            return View(model);
        }
        public ActionResult Animation()
        {
            return View();
        }

        public ActionResult Animation_Temp()
        {

            return View();
        }

        public ActionResult Level()
        {

            return View();
        }

        public ViewResult sicil()
        {
            string IP = Request.UserHostName;
            compName = DetermineCompName(IP);
            return View("ShowSucceed", new ErrorModel { ErrorMessage = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() + "-" + User.Identity.Name.ToString() + "-" + HttpContext.User.Identity.Name + "-" + compName
            });
        }

        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }
    }
}
