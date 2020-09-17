using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data
{
    public interface IDatabaseFactory
    {
        MyContext Create();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private MyContext _database;
        public MyContext Create()
        {
            return _database ?? (_database = new MyContext());
        }
    }
}
