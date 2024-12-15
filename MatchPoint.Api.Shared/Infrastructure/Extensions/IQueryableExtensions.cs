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
        public static IQueryable<T> WithFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
        {
            return query.Where(QuerySpecificationFactory<T>.CreateFilters(filters));
        }

        /// <summary>
        /// Append one OrderBy clause to the <see cref="IQueryable"/> query.
        /// For the first order by key, a OrderBy clause is added.
        /// If sort direction is <see cref="SortDirection.Descending"/>, OrderByDescending is used.
        /// </summary>
        /// <param name="orderBy">
        /// A Dictionary with orderBy property to apply to the query.
        /// </param>
        /// <returns> The original query with a OrderBy clause appended. </returns>
        public static IQueryable<T> WithOrderBy<T>(this IQueryable<T> query, Dictionary<string, SortDirection> orderBy)
        {
            var orderQuery = orderBy.First();
            var sortDirection = orderQuery.Value;
            var orderByExpression = QuerySpecificationFactory<T>.CreateOrderBy(orderQuery.Key);

            query = sortDirection == SortDirection.Descending
                    ? query.OrderByDescending(orderByExpression)
                    : query.OrderBy(orderByExpression);

            return query;
        }
    }
}
