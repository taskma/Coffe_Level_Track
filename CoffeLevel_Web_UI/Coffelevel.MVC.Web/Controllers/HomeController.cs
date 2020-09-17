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
using ChemistDepository.Client.Web.Infrastructure;
using System.Net;
using CoffeLevel.Client.Web.Mailers;

namespace CoffeLevel.Client.Web.Controllers
{
    //[CustomAuthorizationFilter(Roles = "User")]

    public class HomeController : PublicController
    {

        private ICoffeeInfoRepository _coffeeInfoRepository;
        private ICoffeeCalcLevelRepository _coffeeCalcLevelRepository;
        private ICoffeeLevelDecideEngine _coffeeLevelDecideEngine;
        private ICoffeeLevelRepository _coffeeLevelRepository;
        private ICoffeeEmailRepository _coffeeEmailRepository;
        public HomeController(CoffeeLevelRepository coffeeLevelRepository, CoffeeInfoRepository coffeeInfoRepository,
                           CoffeeCalcLevelRepository coffeeCalcLevelRepository, CoffeeLevelDecideEngine coffeeLevelDecideEngine, CoffeeEmailRepository coffeeEmailRepository)
        {
            _coffeeLevelRepository = coffeeLevelRepository;
            _coffeeInfoRepository = coffeeInfoRepository;
            _coffeeCalcLevelRepository = coffeeCalcLevelRepository;
            _coffeeLevelDecideEngine = coffeeLevelDecideEngine;
            _coffeeEmailRepository = coffeeEmailRepository;
        }

        private IUserMailer _userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }


        public ActionResult Index()
        {


            var browser = Request.Browser.Browser.Trim().ToUpperInvariant();

            if (browser == "INTERNETEXPLORER" || browser == "IE")
            {
                return RedirectToAction("WrongIEVersion", "Home");
            }
            return View();
        }

        public ViewResult Email(string email)
        {
            UserMailer.TestMail(email).Send();
            return View("ShowSucceed", new ErrorModel { ErrorMessage = "E-Posta gönderilmiştir !!!" });
        }

        public ViewResult islocal(string email)
        {
            return View("ShowSucceed", new ErrorModel { ErrorMessage = HttpContext.Request.IsLocal.ToString() });
        }

        public ViewResult WrongIEVersion()
        {
            return View();
        }

        public RedirectResult testdirect()
        {

        }
           


        public ActionResult Create(short level)
        {

            var entity = new CoffeeLevelTable();
            entity.Id = Guid.NewGuid();
            entity.level = level;
            entity.temperature = 0;
            entity.humidity = 1;
            entity.time = DateTime.Now;

            _coffeeLevelRepository.Add(entity);

            UnitOfWork.Commit();
            return View();
        }


        public ActionResult IndexJavaScritDeneme()
        {
            return View();
        }

        String compName = null;
        string IP = null;

        public void insertdataEmailTable(String eMail, String compName, bool isDesired, out CoffeeEmailTable eMailEntity)
        {

            //IP = Request.UserHostName;
            //compName = DetermineCompName(IP);

            eMailEntity = new CoffeeEmailTable();
            eMailEntity.computerName = compName;
            eMailEntity.eMailAdress = eMail;
            eMailEntity.isdesired = isDesired;
            _coffeeEmailRepository.Add(eMailEntity);

            UnitOfWork.Commit();
        }

        bool addNumberOfDesire = false;
       
        public JsonResult Desired()
        {
            CoffeeEmailTable eMailEntity = new CoffeeEmailTable();
            CoffeeInfoTable coffeeInfo = new CoffeeInfoTable();
            int totalDesireOfEmailTablo = 0;
            IP = Request.UserHostName;
            compName = DetermineCompName(IP);

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Talep geldi: " + compName));


            if (!_coffeeEmailRepository.hasThisComputerNameDesired(compName))
                {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Talep geldi1: " + compName));
                if (_coffeeEmailRepository.hasThisComputerNameExist(compName))
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Talep geldi2: " + compName));
                        eMailEntity.computerName = compName;
                        _coffeeEmailRepository.changeDesireTrue(compName);
                        UnitOfWork.Commit();
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Talep geldi3: " + compName));
                        eMailEntity.computerName = compName;
                        eMailEntity.Id = Guid.NewGuid();
                        eMailEntity.isdesired = true;
                        _coffeeEmailRepository.Add(eMailEntity);
                        UnitOfWork.Commit();
                    }
                    addNumberOfDesire = true;

                }
           
                totalDesireOfEmailTablo = _coffeeEmailRepository.getTotalDesireOfEmailTable();
              
            
            return Json(totalDesireOfEmailTablo, JsonRequestBehavior.AllowGet);
        }


        


        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

        public void changeCoffeeEmailInfo(CoffeeEmailTable coffeEmailTable)
        {
            CoffeeEmailTable newInfoTable = new CoffeeEmailTable()
            {
                Id = Guid.NewGuid(),
                computerName = coffeEmailTable.computerName,
                eMailAdress = coffeEmailTable.eMailAdress,
                isdesired = true
            };
            
            _coffeeEmailRepository.Delete(coffeEmailTable);
          //  _coffeeEmailRepository.Update(newInfoTable);
            //UnitOfWork.Commit();
            _coffeeEmailRepository.Add(newInfoTable);
           
            UnitOfWork.Commit();
        }


    }




}
