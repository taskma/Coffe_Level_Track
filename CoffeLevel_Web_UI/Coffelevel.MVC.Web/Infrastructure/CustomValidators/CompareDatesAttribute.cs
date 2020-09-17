using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeLevel.Client.Web.Infrastructure.CustomValidators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CompareDatesAttribute : ValidationAttribute
    {
        public String SourceDate { get; set; }
        public String CompareDate { get; set; }

        public CompareDatesAttribute(string source, string match)
        {
            SourceDate = source;
            CompareDate = match;
        }

        public override Boolean IsValid(Object value)
        {
            var objectType = value.GetType();

            var properties = objectType.GetProperties();

            var sourceValue = new object();
            var compareValue = new object();

            var counter = 0;

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name != SourceDate && propertyInfo.Name != CompareDate) continue;
                
                if (counter == 0)
                    sourceValue = propertyInfo.GetValue(value, null);

                if (counter == 1)
                    compareValue = propertyInfo.GetValue(value, null);

                counter++;
                
                if (counter == 2)
                    break;
            }

            if (sourceValue != null && compareValue != null)
            {
                if ((DateTime)sourceValue < (DateTime)compareValue)
                    return true;
            }
            return false;
        }
    }
}