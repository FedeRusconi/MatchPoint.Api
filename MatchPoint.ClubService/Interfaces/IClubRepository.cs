using MatchPoint.Api.Shared.Interfaces;
using MatchPoint.Api.Shared.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubRepository : IRepository
    {
        /// <summary>
        /// Retrieves all clubs from the database.
        /// </summary>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entities. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>An enumerable collection of all <see cref="ClubEntity"/> instances.</returns>
        Task<IEnumerable<ClubEntity>> GetAllAsync(bool trackChanges = true);

        /// <summary>
        /// Retrieves a club by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the club to retrieve.</param>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entity. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>
        /// The <see cref="ClubEntity"/> with the specified identifier, or <c>null</c> if no such entity exists.
        /// </returns>
        Task<ClubEntity?> GetByIdAsync(Guid id, bool trackChanges = true);

        /// <summary>
        /// Adds a new club to the database. Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="club">The <see cref="ClubEntity"/> to add.</param>
        /// <returns> The newly created <see cref="ClubEntity"/>. </returns>
        Task<ClubEntity> CreateAsync(ClubEntity club);

        /// <summary>
        /// Updates an existing club into the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="club">The <see cref="ClubEntity"/> to update.</param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        Task<ClubEntity?> UpdateAsync(ClubEntity club);

        /// <summary>
        /// Updates only provided properties of an existing club into the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="id"> The <see cref="Guid"/> of the club to modify. </param>
        /// <param name="propertyUpdates"> 
        /// An enumerable of <see cref="PropertyUpdate"/> with selected properties to update. 
        /// </param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        Task<ClubEntity> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates);

        /// <summary>
        /// Deletes a club by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the club to delete.</param>
        /// <returns>
        /// True if successful, false if no <see cref="ClubEntity"/> with matching Id was found.
        /// </returns>
        Task<bool> DeleteAsync(Guid id);

    }
}