using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeLevel.Data.Entities
{

    [Table("CoffeeCalcLevelTable")]
    public class CoffeeCalcLevelTable : Entity
    {
        public Int16 level { get; set; }
        public bool isCooking { get; set; }
        public DateTime time { get; set; }
        public bool hasPot { get; set; }
        public bool isOpen { get; set; }
        public bool isBadAvarage { get; set; }

    }
}
