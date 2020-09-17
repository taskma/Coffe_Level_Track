
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
    public class HomeModel : BaseCEModel
    {
        public HtmlString Message { get; set; }
        
    }
}