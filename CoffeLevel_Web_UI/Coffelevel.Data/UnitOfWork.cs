using System;
using CoffeLevel.Data.Repositories;
using CoffeLevel.Data.Entities;
using System.Data.Entity;

namespace CoffeLevel.Data
{
    public interface  IUnitOfWork
    {
        void Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private DbContext Context { get; set; }

        public UnitOfWork(IDatabaseFactory dbFactory)
        {
            Context = dbFactory.Create();
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

    }
}
