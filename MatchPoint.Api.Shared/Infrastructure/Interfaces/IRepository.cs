namespace MatchPoint.Api.Shared.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves the count of <typeparamref name="T"/> that match the provided filters.
        /// If filters are null, a count of all entities is returned.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <param name="filters"> A Dictionary of filters to apply before counting the results. </param>
        /// <returns> The number of <typeparamref name="T"/> that match the filters criteria. </returns>
        public Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null);

        /// <summary>
        /// Retrieves all <typeparamref name="T"/> from the database.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entities. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>An enumerable collection of all <typeparamref name="T"/> instances.</returns>
        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true);

        /// <summary>
        /// Retrieves a <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <param name="trackChanges">
        /// A flag indicating whether to track changes to the returned entity. 
        /// Set to <c>true</c> to enable tracking, or <c>false</c> to disable it for read-only purposes.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/> with the specified identifier or null if not found.
        /// </returns>
        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true);

        /// <summary>
        /// Adds a new <typeparamref name="T"/> to the database.
        /// </summary>
        /// <param name="entity">The <typeparamref name="T"/> to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns> The newly created <typeparamref name="T"/> or null if there was an issue. </returns>
        public Task<T?> CreateAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing <typeparamref name="T"/> in the database.
        /// </summary>
        /// <param name="entity">The <typeparamref name="T"/> to update.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns> The updated <typeparamref name="T"/> or null if not found. </returns>
        public Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="entity">The <typeparamref name="T"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns> The deleted <typeparamref name="T"/> or null if not found. </returns>
        public Task<T?> DeleteAsync(T entity, CancellationToken cancellationToken);
    }
}
