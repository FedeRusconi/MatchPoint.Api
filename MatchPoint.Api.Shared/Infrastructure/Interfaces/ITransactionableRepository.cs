namespace MatchPoint.Api.Shared.Infrastructure.Interfaces
{
    public interface ITransactionableRepository
    {
        /// <summary>
        /// Get or Set if there is a current active transaction.
        /// </summary>
        public bool IsTransactionActive { get; set; }

        /// <summary>
        /// Starts a new soft transaction to ensure multiple operations are saved all at once.
        /// </summary>
        public void BeginTransaction();

        /// <summary>
        /// Commits the current transaction, saving all changes made during the transaction.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        public Task CommitTransactionAsync(CancellationToken cancellationToken);
    }
}