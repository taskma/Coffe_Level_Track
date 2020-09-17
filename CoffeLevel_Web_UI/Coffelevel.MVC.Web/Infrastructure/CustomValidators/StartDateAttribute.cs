using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeLevel.Client.Web.Infrastructure.CustomValidators
{
    public class StartDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            var selectedDay = dt.DayOfWeek;
            
            if (selectedDay == DayOfWeek.Monday)
            {
                return true;
            }
            return false;
        }
    }
}