using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeLevel.Client.Web.Infrastructure.CustomValidators
{
    public class EndDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            var selectedDay = dt.DayOfWeek;

            if (selectedDay == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }
    }
}