using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Shared.Infrastructure.Interfaces
{
    public interface IQueryableRepository<T>
    {
        /// <summary>
        /// Retrieves all <typeparamref name="T"/> from the database that fit the specification.
        /// This method allows for filtering, ordering and paging.
        /// </summary>
        /// <param name="pageNumber"> The number of the page to retrieve, based on page size. </param>
        /// <param name="pageSize"> The number of elements to return. </param>
        /// <param name="filters"> 
        /// A Dictionary containing property name as the key and the filter value. Default is null.
        /// </param>
        /// <param name="orderBy"> 
        /// A Dictionary with property names and <see cref="SortDirection"/>. Default is null.
        /// </param>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entities. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>An instance of <see cref="PagedResponse{T}"/> containing a collection of all <typeparamref name="T"/> instances found.</returns>
        public Task<PagedResponse<T>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null,
            bool trackChanges = true);
    }
}
