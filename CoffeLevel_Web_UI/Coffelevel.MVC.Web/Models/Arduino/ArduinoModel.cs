
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
    public class ArduinoModel : BaseCEModel
    {
        public String Message { get; set; }
        public int levelPercent { get; set; }


    }
}