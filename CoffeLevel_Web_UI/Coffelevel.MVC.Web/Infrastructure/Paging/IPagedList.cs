using System.Collections.Generic;
using System.Web.Routing;

namespace CoffeLevel.Infrastructure.Paging
{
	public interface IPagedList<T> : IList<T>
	{
		int PageCount { get; }
		int TotalItemCount { get; }
		int PageIndex { get; }
		int PageNumber { get; }
		int PageSize { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		bool IsFirstPage { get; }
		bool IsLastPage { get; }
        string OrderCol { get; set; }
        bool OrderAsc { get; set; }
        RouteValueDictionary filterValues { get; set; }
	}
}