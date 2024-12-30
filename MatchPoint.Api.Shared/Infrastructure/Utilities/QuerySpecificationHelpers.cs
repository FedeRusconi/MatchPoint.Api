using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.Api.Shared.Infrastructure.Utilities
{
    public static class QuerySpecificationHelpers
    {
        /// <summary>
        /// Validate page number and size.
        /// </summary>
        /// <typeparam name="T"> This type is included in the IServiceResult response. </typeparam>
        /// <param name="pageNumber"> The page number to validate. </param>
        /// <param name="pageSize"> The page size to validate. </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> with appropriate message if any param is invalid. 
        /// Null if all params are valid.
        /// </returns>
        public static IServiceResult<PagedResponse<T>>? ValidatePagination<T>(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                return ServiceResult<PagedResponse<T>>.Failure(
                    "Page number must be greater than zero.", ServiceResultType.BadRequest);
            }
            if (pageSize < 1 || pageSize > Constants.MaxPageSizeAllowed)
            {
                return ServiceResult<PagedResponse<T>>.Failure(
                    $"Page size must be between 1 and {Constants.MaxPageSizeAllowed}.", ServiceResultType.BadRequest);
            }
            return null;
        }


        /// <summary>
        /// Validate filters.
        /// This checks if all filter keys are valid for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"> This type must include properties for each provided filter. </typeparam>
        /// <param name="filters"> A Dictionary of filters. </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> with appropriate message if any key is invalid. 
        /// Null if all filters are valid.
        /// </returns>
        public static IServiceResult<PagedResponse<T>>? ValidateFilters<T>(Dictionary<string, string>? filters)
        {
            if (filters == null) return null;

            var properties = typeof(T)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.PropertyType, StringComparer.OrdinalIgnoreCase);

            foreach (var filter in filters)
            {
                // Check if the filter key is valid
                if (!properties.TryGetValue(filter.Key, out var propertyType))
                {
                    return ServiceResult<PagedResponse<T>>.Failure(
                        $"Invalid filter key: {filter.Key}.", ServiceResultType.BadRequest);
                }

                // Check if the filter value is compatible with the property type
                if (filter.Value != null && !IsTypeCompatible(filter.Value, propertyType))
                {
                    return ServiceResult<PagedResponse<T>>.Failure(
                        $"Invalid value for filter key: {filter.Key}. Expected type: {propertyType.Name}, but received: {filter.Value.GetType().Name}.",
                        ServiceResultType.BadRequest);
                }
            }

            return null;
        }

        /// <summary>
        /// Validate order by specification.
        /// This checks if all orde by keys are valid for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"> This type must include the property for order-by specification. </typeparam>
        /// <param name="orderBy"> A Dictionary for order by specification. </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> with appropriate message if key is invalid. 
        /// Null if order by is valid.
        /// </returns>
        public static IServiceResult<PagedResponse<T>>? ValidateOrderBy<T>(Dictionary<string, SortDirection>? orderBy)
        {
            if (orderBy == null) return null;
            if (orderBy.Count > 1)
            {
                return ServiceResult<PagedResponse<T>>.Failure(
                    "Only 1 order by clause is allowed.", ServiceResultType.BadRequest);
            }

            var validProperties = typeof(T).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var invalidOrderByKey = orderBy.Keys.FirstOrDefault(k => !validProperties.Contains(k));
            if (invalidOrderByKey != null)
            {
                return ServiceResult<PagedResponse<T>>.Failure(
                    $"Invalid order by key: {invalidOrderByKey}.", ServiceResultType.BadRequest);
            }

            return null;
        }

        /// <summary>
        /// Check if provided value is compatible with target type.
        /// </summary>
        /// <param name="value"> Any value. </param>
        /// <param name="targetType"> The Type to check for compatibility. </param>
        /// <returns> True if value is compatible with target type. False otherwise. </returns>
        private static bool IsTypeCompatible(object value, Type targetType)
        {
            var valueType = value.GetType();

            // Handle nullable types
            if (Nullable.GetUnderlyingType(targetType) is Type underlyingType)
            {
                targetType = underlyingType;
            }

            // Handle type compatibility
            return targetType.IsAssignableFrom(valueType)
                || (targetType == typeof(Guid) && Guid.TryParse(value.ToString(), out _))
                || (targetType.IsEnum && Enum.IsDefined(targetType, value));
        }
    }
}
