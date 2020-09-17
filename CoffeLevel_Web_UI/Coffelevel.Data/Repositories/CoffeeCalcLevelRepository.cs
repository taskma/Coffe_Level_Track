using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Repositories
{
    public interface ICoffeeCalcLevelRepository : IRepository<CoffeeCalcLevelTable>
    {
        //Company GetByName(string Name);
        CoffeeCalcLevelTable GetTodayLast();
        CoffeeCalcLevelTable GetTodayLastBefore();
        IQueryable<CoffeeCalcLevelTable> GetToday();
        CoffeeCalcLevelTable GetTodayLastHasPot();
    }

    public class CoffeeCalcLevelRepository : EFRepository<CoffeeCalcLevelTable>, ICoffeeCalcLevelRepository
    {
        public CoffeeCalcLevelRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {
        }

        public CoffeeCalcLevelTable GetTodayLast()
        {
            var level = this.GetToday().FirstOrDefault();
            return level;
        }

        public CoffeeCalcLevelTable GetTodayLastBefore()
        {
            var level = this.GetToday().Skip(1).FirstOrDefault();
            return level;
        }

        public IQueryable<CoffeeCalcLevelTable> GetToday()
        {
            DateTime todayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var coffeeLevels = this.GetQueryable().Where(p => EntityFunctions.TruncateTime(p.time) >= todayStart).OrderByDescending(o => o.time);
            return coffeeLevels;

        }
        public CoffeeCalcLevelTable GetTodayLastHasPot()
        {
            DateTime todayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var getTodayLast = this.GetTodayLast();
            var todayList = GetToday();
            CoffeeCalcLevelTable lastCalcTable = null;

            foreach (var item in todayList)
            {

                if (item.level != getTodayLast.level) break;
                lastCalcTable = item;
            }
            return lastCalcTable;
        }
    }
    
}
