using System.Linq.Expressions;

namespace MatchPoint.Api.Shared.Infrastructure.Utilities
{
    public static class QuerySpecificationFactory<T>
    {
        /// <summary>
        /// Prepares an expression to add filters to a query.
        /// </summary>
        /// <param name="filters"> A dictionary with the property name as the key, and the value. </param>
        /// <returns> An <see cref="Expression"/> ready to be used in a Where clause for filtering. </returns>
        public static Expression<Func<T, bool>> CreateFilters(Dictionary<string, object> filters)
        {
            if (filters.Count == 0) throw new ArgumentException("Param 'filters' must have at least one entry.");

            Expression<Func<T, bool>> filtersExpression = null!;
            var parameter = Expression.Parameter(typeof(T), "entity");

            foreach (var filter in filters)
            {
                // Prepare property and constant (filter value)
                var property = Expression.Property(parameter, filter.Key);
                var constant = Expression.Constant(filter.Value);
                // Set Equal expression between property value and filter value
                var equalExpression = Expression.Equal(property, constant);

                // If there are no other criteria, set this as first
                if (filtersExpression == null)
                {
                    filtersExpression = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
                }
                // If there are existing criteria, add them with a logical AND.
                else
                {
                    filtersExpression = Expression.Lambda<Func<T, bool>>(
                        Expression.AndAlso(filtersExpression.Body, equalExpression), parameter);
                }
            }

            return filtersExpression;
        }

        /// <summary>
        /// Perpares an expression to add ordering to a query.
        /// </summary>
        /// <param name="propertyName"> The name of the property to use to order by. </param>
        /// <returns> An <see cref="Expression"/> ready to be used in a OrderBy clause for filtering. </returns>
        public static Expression<Func<T, object>> CreateOrderBy(string propertyName)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            var parameter = Expression.Parameter(typeof(T), "entity");
            var propertyExpression = Expression.Property(parameter, propertyName);
            return Expression.Lambda<Func<T, object>>(
                Expression.Convert(propertyExpression, typeof(object)),
                parameter);
        }
    }
}
