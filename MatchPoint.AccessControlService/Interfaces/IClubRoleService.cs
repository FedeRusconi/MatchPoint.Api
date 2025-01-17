using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.AccessControlService.Interfaces
{
    public interface IClubRoleService
    {
        /// <summary>
        /// Retrieve a single <see cref="ClubRoleEntity"/> by its Id.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> that belongs to a <see cref="ClubRoleEntity"/>. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubRoleEntity"/> found
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubRoleEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all <see cref="ClubRoleEntity"/> from the database that fit the specification.
        /// This method allows for filtering, ordering and paging.
        /// </summary>
        /// <param name="pageNumber"> The number of the page to retrieve, based on page size. Default is 1. </param>
        /// <param name="pageSize"> The number of elements to return. Default is 500. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <param name="filters"> 
        /// A Dictionary containing property name as the key and the filter value. Default is null.
        /// </param>
        /// <param name="orderBy"> 
        /// A Dictionary with property names and <see cref="SortDirection"/>. Default is null.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="PagedResponse{T}"/>
        /// with a collection of <see cref="ClubRoleEntity"/> found or details about the error.
        /// </returns>
        public Task<IServiceResult<PagedResponse<ClubRoleEntity>>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null);

        /// <summary>
        /// Adds a new <see cref="ClubRoleEntity"/> to the database.
        /// </summary>
        /// <param name="clubRoleEntity">The <see cref="ClubRoleEntity"/> to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the newly created <see cref="ClubRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubRoleEntity>> CreateAsync(ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing <see cref="ClubRoleEntity"/> in the database.
        /// </summary>
        /// <param name="clubRoleEntity">The <see cref="ClubRoleEntity"/> to update.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="ClubRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubRoleEntity>> UpdateAsync(ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates only provided properties of an existing <see cref="ClubRoleEntity"/> in the database.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> of the club role to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="ClubRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubRoleEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <see cref="ClubRoleEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubRoleEntity"/> deleted
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubRoleEntity>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
