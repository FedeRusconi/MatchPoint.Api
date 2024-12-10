using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubManagementService
    {
        /// <summary>
        /// Retrieve a single <see cref="ClubEntity"/> by its Id.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> that belongs to a <see cref="ClubEntity"/>. </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubEntity"/> found
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubEntity>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all <see cref="ClubEntity"/> from the database that fit the specification.
        /// This method allows for filtering, ordering and paging.
        /// </summary>
        /// <param name="pageNumber"> The number of the page to retrieve, based on page size. Default is 1. </param>
        /// <param name="pageSize"> The number of elements to return. Default is 500. </param>
        /// <param name="filters"> 
        /// A Dictionary containing property name as the key and the filter value. Default is null.
        /// </param>
        /// <param name="orderBy"> 
        /// A KeyValuePair with property name and <see cref="SortDirection"/>. Default is null.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="PagedResponse{T}"/>
        /// with a collection of <see cref="ClubEntity"/> found or details about the error.
        /// </returns>
        public Task<IServiceResult<PagedResponse<ClubEntity>>> GetAllWithSpecificationAsync(
            int pageNumber = 1,
            int pageSize = 500,
            Dictionary<string, object>? filters = null,
            KeyValuePair<string, SortDirection>? orderBy = null);

        /// <summary>
        /// Adds a new <see cref="ClubEntity"/> to the database.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to add.</param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the newly created <see cref="ClubEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubEntity>> CreateAsync(ClubEntity clubEntity);

        /// <summary>
        /// Updates an existing <see cref="ClubEntity"/> in the database.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to update.</param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="ClubEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubEntity>> UpdateAsync(ClubEntity clubEntity);

        /// <summary>
        /// Updates only provided properties of an existing <see cref="ClubEntity"/> in the database.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> of the club to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="ClubEntity"/>
        /// or details about the error.
        /// </returns>
        Task<IServiceResult<ClubEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates);

        /// <summary>
        /// Deletes a <see cref="ClubEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubEntity"/> deleted
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubEntity>> DeleteAsync(Guid id);
    }
}
