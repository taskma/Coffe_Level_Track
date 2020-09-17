using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Repositories
{
    public interface ICoffeeLevelRepository : IRepository<CoffeeLevelTable>
    {
        //Company GetByName(string Name);
        IQueryable<CoffeeLevelTable> GetToday();
        IEnumerable<CoffeeLevelTable> GetAfterRefenceAll(Guid refId, DateTime refTime);
        CoffeeLevelTable GetLastToday();
        IEnumerable<CoffeeLevelTable> GetAfterRefenceWith40Minutes(Guid refId, DateTime refTime);
    }

    public class CoffeeLevelRepository : EFRepository<CoffeeLevelTable>, ICoffeeLevelRepository
    {
        public CoffeeLevelRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {
        }

        public IQueryable<CoffeeLevelTable>  GetToday()
        {
            DateTime todayStart =   new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var coffeeLevels = this.GetQueryable().Where(p => EntityFunctions.TruncateTime(p.time) >= todayStart);
            return coffeeLevels;
        }

        public CoffeeLevelTable GetLastToday()
        {
            DateTime todayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var coffeeLevel = this.GetQueryable().Where(p => EntityFunctions.TruncateTime(p.time) >= todayStart).OrderByDescending(o => o.time).FirstOrDefault();
            return coffeeLevel;
        }

        public IQueryable<CoffeeLevelTable> GetLastHours( int hour)
        {
            DateTime todayStart = DateTime.Now.AddHours(hour * -1);
            var coffeeLevels = this.GetQueryable().Where(p => EntityFunctions.TruncateTime(p.time) >= todayStart);
            return coffeeLevels;
        }

        public IEnumerable <CoffeeLevelTable> GetAfterRefenceAll(Guid refId, DateTime refTime)
        {
            var newAfterTime = refTime.AddSeconds(-10);
            var coffeeLevels = this.GetQueryable().Where(p => p.time >= newAfterTime).OrderBy(o => o.time).AsEnumerable().SkipWhile(s => s.Id != refId);
            return coffeeLevels; 
        }

        public IEnumerable<CoffeeLevelTable> GetAfterRefenceWith40Minutes(Guid refId, DateTime refTime)
        {
            var newAfterTime = refTime.AddSeconds(-10);
            var newBeforeTime = refTime.AddMinutes(40);
            var coffeeLevels = this.GetQueryable().Where(p => p.time >= newAfterTime && p.time <= newBeforeTime).OrderBy(o => o.time).AsEnumerable().SkipWhile(s => s.Id != refId);
            return coffeeLevels;
        }


    }
    
}
