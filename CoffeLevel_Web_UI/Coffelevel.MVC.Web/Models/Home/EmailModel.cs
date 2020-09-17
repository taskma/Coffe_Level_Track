
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
    public class EmailModel 
    {
        public String computerName { get; set; }
        public String eMailAdress { get; set; }
        public bool isdesired { get; set; }


    }
}