using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Repositories
{
    public interface ICoffeeEmailRepository : IRepository<CoffeeEmailTable>
    {
        //Company GetByName(string Name);
        bool hasThisComputerNameDesired(String compName);
        CoffeeEmailTable GetRecord(String compName);
        void ResetRecords();
        bool hasThisComputerNameExist(String compName);
        void changeDesireTrue(String compName);
        int getTotalDesireOfEmailTable();

    }

    public class CoffeeEmailRepository : EFRepository<CoffeeEmailTable>, ICoffeeEmailRepository
    {
        public CoffeeEmailRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {
        }

        public bool hasThisComputerNameDesired(String compName)
        {
            if (this.GetRecord(compName) == null)
                return false;
          
            return this.GetRecord(compName).isdesired;
        }

        public bool hasThisComputerNameExist(String compName)
        {
            if (this.GetRecord(compName) == null)
                return false;

            return true;
        }
        public CoffeeEmailTable GetRecord(String compName)
        {
           
            return this.GetQueryable().Where(p => p.computerName == compName).FirstOrDefault();

        }

        public void ResetRecords()
        {

            var records = this.GetQueryable().Where(p => p.isdesired);
            foreach (var item in records)
            {
                item.isdesired = false;
            }

        }

        public void changeDesireTrue(String compName)
        {

            var record = this.GetQueryable().Where(p => p.computerName == compName).FirstOrDefault();
            record.isdesired = true;


        }

        public int getTotalDesireOfEmailTable()
        {
            var totalDesire = this.GetQueryable().Where(p => p.isdesired == true).Count();
            return totalDesire;

        }

    }
    
}
