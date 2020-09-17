using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeLevel.Data.Entities
{

    public class CoffeeLevelTable : Entity
    {
        public Int16 level { get; set; }
        public DateTime time { get; set; }
        public decimal temperature { get; set; }
        public int humidity { get; set; }
        public Int16 ir0 { get; set; }
        public Int16 ir1 { get; set; }
        public Int16 ir2 { get; set; }
        public Int16 ir3 { get; set; }
        public Int16 ir4 { get; set; }

        public virtual ICollection<CoffeeInfoTable> coffeeInfos { get; set; }
        public CoffeeLevelTable()
        {
            this.coffeeInfos = new HashSet<CoffeeInfoTable>();
        }

    }
}
