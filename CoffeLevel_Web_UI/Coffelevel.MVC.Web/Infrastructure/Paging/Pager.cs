using CoffeLevel.Client.Web.Infrastructure;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace CoffeLevel.Infrastructure.Paging
{
	public class Pager
	{
		private ViewContext viewContext;
		private readonly int pageSize;
		private readonly int currentPage;
		private readonly int totalItemCount;
		private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
		private readonly AjaxOptions ajaxOptions;
        private readonly bool includeItemsPerPage;

		public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions, bool includeItemsPerPage = true)
		{
			this.viewContext = viewContext;
			this.pageSize = pageSize;
			this.currentPage = currentPage;
			this.totalItemCount = totalItemCount;
			this.linkWithoutPageValuesDictionary = valuesDictionary;
			this.ajaxOptions = ajaxOptions;
            this.includeItemsPerPage = includeItemsPerPage;
		}

		public HtmlString RenderHtml()
		{
			var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
			const int nrOfPagesToDisplay = 10;

			var sb = new StringBuilder("<ul class='pagination'>");

            if (pageCount > 1)
            {
                // Previous
                sb.Append(currentPage > 1 ? GeneratePageLink("Önceki", currentPage - 1, "") : "<li><a href='javascript:void(0)' class='disabled'>Önceki</a></li>");

                var start = 1;
                var end = pageCount;

                if (pageCount > nrOfPagesToDisplay)
                {
                    var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                    var below = (currentPage - middle);
                    var above = (currentPage + middle);

                    if (below < 4)
                    {
                        above = nrOfPagesToDisplay;
                        below = 1;
                    }
                    else if (above > (pageCount - 4))
                    {
                        above = pageCount;
                        below = (pageCount - nrOfPagesToDisplay);
                    }

                    start = below;
                    end = above;
                }

                if (start > 3)
                {
                    sb.Append(GeneratePageLink("1", 1, ""));
                    sb.Append(GeneratePageLink("2", 2, ""));
                    sb.Append(GeneratePageLink("...", 3, ""));
                    //sb.Append("<li>...</li>");
                }

                for (var i = start; i <= end; i++)
                {
                    if (i == currentPage || (currentPage <= 0 && i == 0))
                    {
                        //sb.AppendFormat("<li class='active'>{0}</li>", i);
                        sb.Append(GeneratePageLink(i.ToString(), i, "active pageActiveNumber"));
                    }
                    else
                    {
                        sb.Append(GeneratePageLink(i.ToString(), i, ""));
                    }
                }
                if (end < (pageCount - 3))
                {
                    //sb.Append("...");
                    sb.Append(GeneratePageLink("...", pageCount - 2, ""));
                    sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1, ""));
                    sb.Append(GeneratePageLink(pageCount.ToString(), pageCount, ""));
                }

                // Next
                sb.Append(currentPage < pageCount ? GeneratePageLink("Sonraki", (currentPage + 1), "") : "<li><a href='javascript:void(0)' class='disabled'>Sonraki</a></li>");


                
            }


            //Total item count
            string actionName = linkWithoutPageValuesDictionary["action"].ToString();
            if ( actionName == "_SuggestionListGrid")
            {
                sb.Append(string.Format("<span class=\"totalItemCount\">Toplamda {0} adet öneriniz bulunmaktadır</span>", totalItemCount));
            }
            else if(actionName == "_SuggestionGrid")
            {
                sb.Append(string.Format("<span class=\"totalItemCount\">Toplamda {0} adet öneri bulunmuştur</span>", totalItemCount));
            }
            else
            {
                sb.Append(string.Format("</ul><span class='totalItemCount pull-right'><span class='badge' style='font-size:18px'>{0}</span> adet kayıt bulundu</span>", totalItemCount));
            }
			return new HtmlString(sb.ToString());
		}

		private string GeneratePageLink(string linkText, int pageNumber, string cssClass)
		{
			var pageLinkValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary) { { "page", pageNumber } };

			// To be sure we get the right route, ensure the controller and action are specified.
			var routeDataValues = viewContext.RequestContext.RouteData.Values;
			if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
			{
				pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
			}
			if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
			{
				pageLinkValueDictionary.Add("action", routeDataValues["action"]);
			}
            if (!pageLinkValueDictionary.ContainsKey("id") && routeDataValues.ContainsKey("id"))
            {
                pageLinkValueDictionary.Add("id", routeDataValues["id"]);
            }

			// 'Render' virtual path.
			var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

			if (virtualPathForArea == null)
				return null;

			var stringBuilder = new StringBuilder("<li class='" + cssClass + "'><a");

			if (ajaxOptions != null)
				foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())
					stringBuilder.AppendFormat(" {0}='{1}'", ajaxOption.Key, ajaxOption.Value);

			stringBuilder.AppendFormat(" href='{0}' >{1}</a></li>", virtualPathForArea.VirtualPath, linkText);

			return stringBuilder.ToString();
		}


	}
}