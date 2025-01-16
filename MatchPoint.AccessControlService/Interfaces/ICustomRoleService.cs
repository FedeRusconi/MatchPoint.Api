using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.AccessControlService.Interfaces
{
    public interface ICustomRoleService
    {
        /// <summary>
        /// Retrieve a single <see cref="CustomRoleEntity"/> by its Id.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> that belongs to a <see cref="CustomRoleEntity"/>. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="CustomRoleEntity"/> found
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<CustomRoleEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all <see cref="CustomRoleEntity"/> from the database.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// A <see cref="IServiceResult{T}"/> class containing an IEnumerable of <see cref="CustomRoleEntity"/>
        /// found or details about the error.
        /// </returns>
        public Task<IServiceResult<IEnumerable<CustomRoleEntity>>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new <see cref="CustomRoleEntity"/> to the database.
        /// </summary>
        /// <param name="customRoleEntity">The <see cref="CustomRoleEntity"/> to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the newly created <see cref="CustomRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<CustomRoleEntity>> CreateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing <see cref="CustomRoleEntity"/> in the database.
        /// </summary>
        /// <param name="customRoleEntity">The <see cref="CustomRoleEntity"/> to update.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="CustomRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<CustomRoleEntity>> UpdateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates only provided properties of an existing <see cref="CustomRoleEntity"/> in the database.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> of the custom role to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the updated <see cref="CustomRoleEntity"/>
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<CustomRoleEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <see cref="CustomRoleEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id"> The unique identifier of the entity to delete. </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, allowing the operation to be stopped if no longer needed.
        /// </param>
        /// <returns> 
        /// A <see cref="IServiceResult{T}"/> class containing the <see cref="CustomRoleEntity"/> deleted
        /// or details about the error.
        /// </returns>
        public Task<IServiceResult<CustomRoleEntity>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
