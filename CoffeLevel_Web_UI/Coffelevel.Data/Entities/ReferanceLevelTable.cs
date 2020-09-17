using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeLevel.Data.Entities
{

    public class ReferanceLevelTable : Entity
    {
        public DateTime time { get; set; }
        public Guid levelId { get; set; }

    }
}
