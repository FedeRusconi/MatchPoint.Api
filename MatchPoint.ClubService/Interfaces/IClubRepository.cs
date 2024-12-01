using MatchPoint.Api.Shared.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubRepository : IRepository
    {
        /// <summary>
        /// Adds a new club to the database. Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="club">The <see cref="ClubEntity"/> to add.</param>
        /// <returns> The newly created <see cref="ClubEntity"/>. </returns>
        Task<ClubEntity> CreateAsync(ClubEntity club);

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
        /// Deletes a club by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the club to delete.</param>
        /// <returns>
        /// True if successful, false if no <see cref="ClubEntity"/> with matching Id was found.
        /// </returns>
        Task<bool> DeleteAsync(Guid id);

    }
}