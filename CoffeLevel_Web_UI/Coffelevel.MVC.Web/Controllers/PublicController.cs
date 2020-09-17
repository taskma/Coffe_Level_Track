using CoffeLevel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeLevel.Client.Web.Infrastructure;

namespace CoffeLevel.Client.Web.Controllers
{
    public class PublicController : Controller
    {
        public IUnitOfWork UnitOfWork;
        public PublicController()
        {
            _PageSize = 10;
            this.UnitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();
        }
        private int _PageSize;

        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
        
    }
}
