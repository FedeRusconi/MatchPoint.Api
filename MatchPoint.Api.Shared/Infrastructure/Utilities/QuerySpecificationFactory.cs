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
        public static Expression<Func<T, bool>> CreateFilters(Dictionary<string, string> filters)
        {
            if (filters.Count == 0) throw new ArgumentException("Param 'filters' must have at least one entry.");

            Expression? filtersExpression = null;
            var parameter = Expression.Parameter(typeof(T), "entity");

            foreach (var filter in filters)
            {
                // Get the property type
                var property = Expression.Property(parameter, filter.Key);
                var propertyType = property.Type;

                // Parse the filter value into the appropriate type
                object? parsedValue;
                try
                {
                    parsedValue = ConvertValue(filter.Value, propertyType);
                }
                catch
                {
                    throw new ArgumentException($"Unable to convert filter value '{filter.Value}' to type '{propertyType}'.");
                }

                var constant = Expression.Constant(parsedValue, propertyType);

                // Create the equality expression
                var equalExpression = Expression.Equal(property, constant);

                // Combine with existing filters using AND
                filtersExpression = filtersExpression == null
                    ? equalExpression
                    : Expression.AndAlso(filtersExpression, equalExpression);
            }

            // Wrap in a lambda
            return Expression.Lambda<Func<T, bool>>(filtersExpression!, parameter);
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

        /// <summary>
        /// Convert the given value to the provided target type.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="targetType"> The Type to convert the value to. </param>
        /// <returns> The converted value. </returns>
        private static object? ConvertValue(string? value, Type targetType)
        {
            if (value == null) return null;

            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value, true);
            }

            if (targetType == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }

            if (targetType == typeof(Guid))
            {
                return Guid.Parse(value);
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
