using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Providers.Entities;
using System.Globalization;
using System.Collections;



namespace CoffeLevel.Client.Web.Infrastructure
{
    public  interface IRessManager
    {
         string GetPageResource(string key);
         string GetModelName { get;  }
         ICollection<string> GetResourceCitys();
    }

    public static class RessManager 
    {

       public static string _Key;
       private static string _oldRessFile;
       private static string _newRessFile;


       public static string GetPageResource(string key)
       {
  
           _newRessFile = "CoffeLevel.Client.Web.Resources.Views." + GetControllerName + "." + GetControllerName + "Strings";
           return ResourceManager.GetString(key);

       }

       public static string GetGeneralResource(string key)
       {

           _newRessFile = "CoffeLevel.Client.Web.Resources.General";
           return ResourceManager.GetString(key);

       }

       public static ICollection<string> GetResourceCitys()
       {

           _newRessFile = "CoffeLevel.Client.Web.Resources.Citys";

           ResourceSet resourceSet = ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
           ICollection<string> citys = new List<string>();


          foreach (DictionaryEntry entry in resourceSet)
          {
                citys.Add(entry.Value.ToString());
          }
          return citys;
       }

        public static string GetControllerName
        {
            get
            {

                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                if (routeValues != null)
                {
                    //if (routeValues.ContainsKey("action"))
                    //{
                    //    var actionName = routeValues["action"].ToString();
                    //}
                    if (routeValues.ContainsKey("controller"))
                    {
                       return routeValues["controller"].ToString();
                    }
                    
                }
                return "";

            }
           
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        internal static  System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((_newRessFile != _oldRessFile) || (_newRessFile == _oldRessFile && object.ReferenceEquals(resourceMan, null)))
                {
                        
                        global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(_newRessFile, typeof(RessManager).Assembly);
                        resourceMan = temp;
                        _oldRessFile = _newRessFile;
                }

               
                return resourceMan;
            }
        }






       
    }
}