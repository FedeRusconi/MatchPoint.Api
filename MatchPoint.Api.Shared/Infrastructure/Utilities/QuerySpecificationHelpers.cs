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
        public static IServiceResult<PagedResponse<T>>? ValidateFilters<T>(Dictionary<string, object>? filters)
        {
            if (filters == null) return null;

            var validProperties = typeof(T).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var invalidFilterKey = filters.Keys.FirstOrDefault(k => !validProperties.Contains(k));
            if (invalidFilterKey != null)
            {
                return ServiceResult<PagedResponse<T>>.Failure(
                    $"Invalid filter key: {invalidFilterKey}.", ServiceResultType.BadRequest);
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
    }
}
