namespace MatchPoint.Api.Shared.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves the count of <typeparamref name="T"/> that match the provided filters.
        /// If filters are null, a count of all entities is returned.
        /// </summary>
        /// <param name="filters"> A Dictionary of filters to apply before counting the results. </param>
        /// <returns> The number of <typeparamref name="T"/> that match the filters criteria. </returns>
        Task<int> CountAsync(Dictionary<string, object>? filters = null);

        /// <summary>
        /// Retrieves all <typeparamref name="T"/> from the database.
        /// </summary>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entities. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>An enumerable collection of all <typeparamref name="T"/> instances.</returns>
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges = true);

        /// <summary>
        /// Retrieves a <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entity. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/> with the specified identifier.
        /// </returns>
        Task<T> GetByIdAsync(Guid id, bool trackChanges = true);

        /// <summary>
        /// Adds a new <typeparamref name="T"/> to the database.
        /// </summary>
        /// <param name="entity">The <typeparamref name="T"/> to add.</param>
        /// <returns> The newly created <typeparamref name="T"/>. </returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing <typeparamref name="T"/> in the database.
        /// </summary>
        /// <param name="entity">The <typeparamref name="T"/> to update.</param>
        /// <returns> The updated <typeparamref name="T"/>. </returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes a <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>
        /// True if successful, false if no <typeparamref name="T"/> with matching Id was found.
        /// </returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
