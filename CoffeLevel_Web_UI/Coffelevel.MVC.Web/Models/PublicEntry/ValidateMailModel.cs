using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeLevel.Client.Web.Models
{
    public class ValidateMailModel : BaseCEModel
    {
        public bool Issucced { get; set; }
        public string ErrorMessage { get; set; }

    }
}