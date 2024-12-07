using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubManagementService
    {
        /// <summary>
        /// Retrieve a single <see cref="ClubEntity"/> by its Id.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> that belongs to a <see cref="ClubEntity"/>. </param>
        /// <returns> The <see cref="ClubEntity"/> found. </returns>
        public Task<ClubEntity> GetByIdAsync(Guid id);

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
        /// <returns>An instance of <see cref="PagedResponse{T}"/> containing a collection of all <see cref="ClubEntity"/> instances found.</returns>
        public Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(
            int pageNumber = 1,
            int pageSize = 500,
            Dictionary<string, object>? filters = null,
            KeyValuePair<string, SortDirection>? orderBy = null);

        /// <summary>
        /// Adds a new <see cref="ClubEntity"/> to the database.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to add.</param>
        /// <returns> The newly created <see cref="ClubEntity"/>. </returns>
        public Task<ClubEntity> CreateAsync(ClubEntity clubEntity);

        /// <summary>
        /// Updates an existing <see cref="ClubEntity"/> in the database.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to update.</param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        public Task<ClubEntity> UpdateAsync(ClubEntity clubEntity);

        /// <summary>
        /// Updates only provided properties of an existing <see cref="ClubEntity"/> in the database.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> of the club to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        Task<ClubEntity> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates);

        /// <summary>
        /// Deletes a <see cref="ClubEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <returns>
        /// True if successful, false if no <see cref="ClubEntity"/> with matching Id was found.
        /// </returns>
        public Task<bool> DeleteAsync(Guid id);
    }
}
