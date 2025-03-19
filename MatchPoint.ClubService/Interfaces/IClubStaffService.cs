using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubStaffService
    {
        /// <summary>
        /// Retrieve a single <see cref="ClubStaffEntity"/> by its Id.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> that belongs to the parent <see cref="ClubEntity"/>. </param>
        /// <param name="id"> The <see cref="Guid"/> that belongs to a <see cref="ClubStaffEntity"/>. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubStaffEntity"/> found
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubStaffEntity>> GetByIdAsync(Guid clubId, Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all <see cref="ClubStaffEntity"/> from the database that fit the specification.
        /// This method allows for filtering, ordering and paging.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> that belongs to the parent <see cref="ClubEntity"/>. </param>
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
        /// with a collection of <see cref="ClubStaffEntity"/> found or details about the error.
        /// </returns>
        public Task<IServiceResult<PagedResponse<ClubStaffEntity>>> GetAllWithSpecificationAsync(
            Guid clubId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null);

        /// <summary>
        /// Adds a new <see cref="ClubStaffEntity"/> to the database.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> that belongs to the parent <see cref="ClubEntity"/>. </param>
        /// <param name="clubStaffEntity">The <see cref="ClubStaffEntity"/> to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the newly created <see cref="ClubStaffEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubStaffEntity>> CreateAsync(
            Guid clubId, ClubStaffEntity clubStaffEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates only provided properties of an existing <see cref="ClubStaffEntity"/> in the database.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> that belongs to the parent <see cref="ClubEntity"/>. </param>
        /// <param name="id"> The <see cref="Guid"/> of the club staff to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="ClubStaffEntity"/>
        /// or details about the error.
        /// </returns>
        Task<IServiceResult<ClubStaffEntity>> PatchAsync(
            Guid clubId, Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <see cref="ClubStaffEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> that belongs to the parent <see cref="ClubEntity"/>. </param>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="ClubStaffEntity"/> deleted
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<ClubStaffEntity>> DeleteAsync(Guid clubId, Guid id, CancellationToken cancellationToken);
    }
}
