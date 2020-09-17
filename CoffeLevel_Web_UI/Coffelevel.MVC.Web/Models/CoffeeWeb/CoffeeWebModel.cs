
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
    public class CoffeeWebModel : BaseCEModel
    {
        public HtmlString Message { get; set; }
        

        public IQueryable<CoffeeLevelTable> coffeeLevelList { get; set; }
        public IQueryable<CoffeeCalcLevelTable> coffeeCalcLevelList { get; set; }

        public IQueryable<CoffeeEmailTable> coffeeEmailList { get; set; }

        public CoffeeLevelTable referanceTable { get; set; }

        public IEnumerable<CoffeeLevelTable> avarageLevelList { get; set; }
        public IQueryable<ReferanceLevelTable> referanceLevelList { get; set; }


    }
}