using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Repositories
{
    public interface ICoffeeInfoRepository : IRepository<CoffeeInfoTable>
    {
        CoffeeInfoTable GetInfo();
    }

    public class CoffeeInfoRepository : EFRepository<CoffeeInfoTable>, ICoffeeInfoRepository
    {
        public CoffeeInfoRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {
        }

        public CoffeeInfoTable GetInfo()
        {
            var info = this.GetQueryable().FirstOrDefault();
            return info;
        }
    }
    
}
