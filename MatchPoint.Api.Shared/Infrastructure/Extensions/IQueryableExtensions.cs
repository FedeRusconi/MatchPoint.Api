using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Infrastructure.Utilities;

namespace MatchPoint.Api.Shared.Infrastructure.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Append a Where clause to the <see cref="IQueryable"/> query with one condition
        /// for each filter within the dictionary.
        /// </summary>
        /// <param name="filters">
        /// A Dictionary of filters to apply to the query.
        /// </param>
        /// <returns> The original query with a Where clause appended. </returns>
        public static IQueryable<T> WithFilters<T>(this IQueryable<T> query, Dictionary<string, object> filters)
        {
            return query.Where(QuerySpecificationFactory<T>.CreateFilters(filters));
        }

        /// <summary>
        /// Append one or more OrderBy clause to the <see cref="IQueryable"/> query.
        /// For each order by key, a OrderBy (if first) or ThenBy clause is added.
        /// If sort direction is <see cref="SortDirection.Descending"/>, OrderByDescending and ThenByDescending are used.
        /// </summary>
        /// <param name="orderBy">
        /// A Dictionary of orderBy properties to apply to the query.
        /// </param>
        /// <returns> The original query with one or more OrderBy clauses appended. </returns>
        public static IQueryable<T> WithOrderBy<T>(this IQueryable<T> query, Dictionary<string, SortDirection> orderBy)
        {
            int i = 0;
            foreach (var orderQuery in orderBy)
            {
                var sortDirection = orderQuery.Value;
                var orderByExpression = QuerySpecificationFactory<T>.CreateOrderBy(orderQuery.Key);

                if (i == 0)
                {
                    query = sortDirection == SortDirection.Descending
                        ? query.OrderByDescending(orderByExpression)
                        : query.OrderBy(orderByExpression);
                }
                else
                {
                    query = sortDirection == SortDirection.Descending
                        ? ((IOrderedQueryable<T>)query).ThenByDescending(orderByExpression)
                        : ((IOrderedQueryable<T>)query).ThenBy(orderByExpression);
                }
                i++;                
            }
            
            return query;
        }
    }
}
