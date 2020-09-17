using CoffeLevel.Data.Entities;
using CoffeLevel.Client.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace CoffeLevel.Client.Web.HelperMethods
{
    public static class AllHelpers
    {
        public static string GetControllerName( this HtmlHelper html)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            string name = "";
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("controller"))
                {
                    name = routeValues["controller"].ToString();
                }
               
            }
            return name;
        }

        public static string GetActionName(this HtmlHelper html)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            string name = "";
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    name = routeValues["action"].ToString();
                }

            }
            return name;
        }
      

        public static HtmlString GetEmails(this HtmlHelper html, ICollection<string> IncomingEmails)
        {
            return  new HtmlString(FormatEmails(IncomingEmails));
        }

      

        public static HtmlString GetEmails(this HtmlHelper html, params string[] IncomingEmails)
        {
            return new HtmlString(FormatEmails(IncomingEmails));
        }

        private static string FormatEmails(ICollection<string> IncomingEmails)
        {
            string Emails = "";
            foreach (var item in IncomingEmails)
            {
                string each = item.ToSafeTrim().Replace(";", ", </br>");
                if (!string.IsNullOrEmpty(each)) Emails += Emails == "" ? each : ", </br>" + each;
            }
            return Emails == "" ? "-" : Emails;
        }

        public static string NullFormatter(this HtmlHelper html, string word)
        {
            if (string.IsNullOrEmpty(word)) return "-";
            return word;
        }

        public static string ChemistFormatter(this HtmlHelper html, string word)
        {
            return string.Concat(word, word.ToLower().Contains("eczane") ? " " : " Eczanesi ");
        }

       
    }
}