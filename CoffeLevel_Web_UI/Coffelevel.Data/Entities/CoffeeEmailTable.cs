using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeLevel.Data.Entities
{

    [Table("CoffeeEmailTable")]
    public class CoffeeEmailTable : Entity
    {
        public String computerName { get; set; }
        public String eMailAdress { get; set; }
        public bool isdesired { get; set; }

    }
}
