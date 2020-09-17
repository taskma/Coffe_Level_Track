using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public class Entity : IEntity
    {
        public Guid Id { get; set; }
    }
}
