
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeLevel.Data.Entities;

namespace CoffeLevel.Client.Web.Models
{
    public class LevelModel 
    {
        public int coffeeLevelId { get; set; }
        //public int coffeeLevel { get; set; }
        //public int coffeeRequest { get; set; }


        public Int16 level { get; set; }
        public bool isCooking { get; set; }
        public String time { get; set; }
        public bool hasPot { get; set; }
        public bool isOpen { get; set; }
        public Int16 totalDesire { get; set; }
        public String coffeeStatus { get; set; }


    }
}