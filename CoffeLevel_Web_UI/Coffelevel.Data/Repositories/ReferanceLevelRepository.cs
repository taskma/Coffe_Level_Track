using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Repositories
{
    public interface IReferanceLevelRepository : IRepository<ReferanceLevelTable>
    {
        


    }

    public class ReferanceLevelRepository : EFRepository<ReferanceLevelTable>, IReferanceLevelRepository
    {
        public ReferanceLevelRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {
        }

        

    }
    
}
