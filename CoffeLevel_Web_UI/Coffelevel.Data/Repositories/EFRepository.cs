using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CoffeLevel.Data.Repositories
{
    public class EFRepository<TEntity> :
        IRepository<TEntity> where TEntity : Entity
    {

        private MyContext context;
        private DbSet<TEntity> _entitySet;

        public EFRepository(IDatabaseFactory dbFactory)
        {
            this.context = dbFactory.Create();
            this._entitySet = this.context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entitySet.ToList();
        }

        public TEntity GetById(Guid id)
        {
            return _entitySet.Find(id);
        }

        public void Add(TEntity entity)
        {
            _entitySet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _entitySet.Attach(entity);
        }

        public void Delete(TEntity entity)
        {
            var entityToRemove = GetById(entity.Id);
            _entitySet.Remove(entityToRemove);
        }

        public void DeleteAll()
        {
            foreach (var item in _entitySet.ToList())
            {
                _entitySet.Remove(item);
            }
            
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _entitySet.AsQueryable();
        }
    }
}
