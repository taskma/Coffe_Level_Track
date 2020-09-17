using CoffeLevel.Client.Web.DependencyResolution;
using CoffeLevel.Client.Web;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using CoffeLevel.Data;

namespace CoffeLevel.Client.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Database.SetInitializer<MyContext>(null);

            Database.SetInitializer<MyContext>(new DataInitializer());

        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //var newCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            //newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            //newCulture.DateTimeFormat.DateSeparator = "/";
            //Thread.CurrentThread.CurrentCulture = newCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        }

    }
}