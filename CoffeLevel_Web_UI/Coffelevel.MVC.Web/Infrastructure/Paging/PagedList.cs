using CoffeLevel.Client.Web.Infrastructure.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace CoffeLevel.Infrastructure.Paging
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {


        public PagedList(IEnumerable<T> source, int index, int pageSize, int? totalCount = null)
            : this(source.AsQueryable(), index, pageSize, totalCount)
        {
        }

        public PagedList(IEnumerable<T> source, int index, int pageSize, string orderCol, bool? orderAsc, int? totalCount = null)
            : this(source.AsQueryable(), index, pageSize, orderCol, orderAsc, totalCount)
        {
        }

        public PagedList(IQueryable<T> source, int index, int pageSize, string orderCol, bool? orderAsc, int? totalCount = null)
            : this(source, index, pageSize, totalCount)
        {
            OrderCol = orderCol;
            OrderAsc = orderAsc.HasValue ? orderAsc.Value : false;
        }

        public PagedList(IQueryable<T> source, int index, int pageSize, int? totalCount = null)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Value can not be below 0.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Value can not be less than 1.");

            if (source == null)
                source = new List<T>().AsQueryable();

            int realTotalCount = 0;

            if (!totalCount.HasValue)
                realTotalCount = source.Count();

            PageSize = pageSize;
            PageIndex = index;
            TotalItemCount = totalCount.HasValue ? totalCount.Value : realTotalCount;
            PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;

            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));

            if (TotalItemCount <= 0)
                return;

            var realTotalPages = (int)Math.Ceiling(realTotalCount / (double)PageSize);

            if (realTotalCount < TotalItemCount && realTotalPages <= PageIndex)
                AddRange(source.Skip((realTotalPages - 1) * PageSize).Take(PageSize));
            else
                AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));


        }

        public PagedList(IQueryable<T> source, int index, int pageSize, IFilterModel filterModel, int? totalCount = null)
            : this(source, index, pageSize, totalCount)
        {
            if (filterModel != null)
            {
                RouteValueDictionary _filterValues = new RouteValueDictionary();
                var properties = filterModel.GetType().GetProperties();

                properties.ToList().ForEach(info =>
                {
                    if (info.PropertyType.Name.ToUpper().IndexOf("IENUMERABLE") < 0)
                        _filterValues.Add(info.Name, info.GetValue(filterModel, null));
                });
                this.filterValues = _filterValues;
            }
        }

        #region IPagedList Members

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageNumber { get { return PageIndex + 1; } }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
        public string OrderCol { get; set; }
        public bool OrderAsc { get; set; }
        public RouteValueDictionary filterValues { get; set; }

        #endregion
    }
}