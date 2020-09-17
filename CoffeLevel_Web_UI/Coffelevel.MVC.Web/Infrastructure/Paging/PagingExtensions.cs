using CoffeLevel.Client.Web.Infrastructure.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace CoffeLevel.Infrastructure.Paging
{
    public static class PagingExtensions
    {
        #region AjaxHelper extensions

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, null, ajaxOptions, includeItemsPerPage);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, actionName, null, ajaxOptions, includeItemsPerPage);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, object values, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values), ajaxOptions, includeItemsPerPage);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values), ajaxOptions, includeItemsPerPage);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary, ajaxOptions, includeItemsPerPage);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            var pager = new Pager(ajaxHelper.ViewContext, pageSize, currentPage, totalItemCount, valuesDictionary, ajaxOptions, includeItemsPerPage);
            return pager.RenderHtml();
        }

        #endregion

        #region HtmlHelper extensions

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, bool includeItemsPerPage = true)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, null, includeItemsPerPage);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, bool includeItemsPerPage = true)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, null, includeItemsPerPage);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, object values, bool includeItemsPerPage = true)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values), includeItemsPerPage);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values, bool includeItemsPerPage = true)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values), includeItemsPerPage);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, bool includeItemsPerPage = true)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary, includeItemsPerPage);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, RouteValueDictionary valuesDictionary, bool includeItemsPerPage = true)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            var pager = new Pager(htmlHelper.ViewContext, pageSize, currentPage, totalItemCount, valuesDictionary, null, includeItemsPerPage);
            return pager.RenderHtml();
        }

        #endregion

        #region IQueryable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, string orderCol, bool? orderAsc, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, orderCol, orderAsc, totalCount);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, IFilterModel filterModel, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, filterModel, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, string orderCol, bool? orderAsc, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, orderCol, orderAsc, totalCount);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}