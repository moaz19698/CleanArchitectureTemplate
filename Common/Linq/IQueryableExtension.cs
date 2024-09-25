using CleanArchitecture.Common.Linq.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Common.Linq
{
    public static class IQueryableExtension
    {
        public static (List<T>, int totalCount) ToPagedList<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            int totalCount = query.Count();
            var result = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return (result, totalCount);
        }

        public static async Task<(List<T> items, int totalCount)> ToPagedListAsync<T>(this IQueryable<T> query,
                                                                                      int pageIndex,
                                                                                      int pageSize,
                                                                                      CancellationToken cancellationToken = default) where T : class
        {
            int totalCount = await query.CountAsync(cancellationToken);
            var result = await query.Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(cancellationToken);

            return (result, totalCount);
        }
        public static async Task<(List<TResult> items, int totalCount)> ToPagedListAsync<T, TResult>(this IQueryable<T> query,
                                                                                                     int pageIndex,
                                                                                                     int pageSize,
                                                                                                     Expression<Func<T, TResult>> selector,
                                                                                                     CancellationToken cancellationToken = default) where T : class
        {
            int totalCount = await query.CountAsync(cancellationToken);
            var result = await query.Select(selector).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return (result, totalCount);
        }

        /// <summary>
        /// The WhereIf to assert from condition before applying filter
        /// </summary>
        /// <param name="IfCondition">the precondition to apply the filter or not</param>
        /// <param name="filter"></param>
        /// <returns></returns>

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query,
                                               bool IfCondition,
                                               Expression<Func<T, bool>> filter) where T : class
        {
            if (IfCondition && filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }

        private static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> source,
                                                            bool isFirstLevel,
                                                            string propertyName,
                                                            ListSortDirection descending = ListSortDirection.Ascending)
        {
            source = isFirstLevel ? OrderFirstLevel(source, propertyName, descending)
                                  : OrderLastLevel(source, propertyName, descending);

            IOrderedQueryable<T> OrderFirstLevel(IQueryable<T> source,
                                                 string propertyName,
                                                 ListSortDirection descending) => descending == ListSortDirection.Ascending
                                                    ? source.OrderBy(o => EF.Property<object>(o!, propertyName))
                                                    : source.OrderByDescending(o => EF.Property<object>(o!, propertyName));

            IOrderedQueryable<T> OrderLastLevel(IQueryable<T> source,
                                                string propertyName,
                                                ListSortDirection descending) => descending == ListSortDirection.Ascending
                                                    ? ((IOrderedQueryable<T>)source).ThenBy(o => EF.Property<object>(o!, propertyName))
                                                    : ((IOrderedQueryable<T>)source).ThenByDescending(o => EF.Property<object>(o!, propertyName));

            return source;
        }

        public static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> source,
                                                             string propertyName,
                                                             ListSortDirection descending = ListSortDirection.Ascending)
        {
            return ((IOrderedQueryable<T>)source.DynamicOrderBy(true, propertyName, descending)).ThenBy(o => EF.Property<object>(o!, "Id"));
        }

        public static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> source, List<DynamicOrderFields> orderPropertyList)
        {
            if (orderPropertyList.Any())
            {
                var orderField = orderPropertyList.OrderBy(o => o.SortNumber).FirstOrDefault();
                source = source.DynamicOrderBy(true, orderField!.PropertyName, orderField.SortDirection);
                orderPropertyList.RemoveAt(0);

                return orderPropertyList.OrderBy(o => o.SortNumber).Aggregate(source,
                                                        (source, o) => source.DynamicOrderBy(false, o.PropertyName, o.SortDirection));
            }
            return source;
        }
    }
}
