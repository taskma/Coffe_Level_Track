using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeLevel.Data.Entities
{

    [Table("CoffeeInfoTable")]
    public class CoffeeInfoTable : Entity
    {
        public Guid? referenceLevelID { get; set; }
        public Guid? referenceNextLevelID { get; set; }
        public Guid? referenceTempID { get; set; }
        public Int16 totalDesire { get; set; }
        public DateTime lastDesireTime { get; set; }

        [ForeignKey("referenceLevelID")]
        public virtual CoffeeLevelTable referenceLevel { get; set; }

        [ForeignKey("referenceNextLevelID")]
        public virtual CoffeeLevelTable referenceNextLevel { get; set; }

        [ForeignKey("referenceTempID")]
        public virtual CoffeeLevelTable referenceTempLevel { get; set; }



    }
}
