namespace MatchPoint.Api.Shared.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// Get or Set if there is a current active transaction.
        /// </summary>
        bool IsTransactionActive { get; set; }

        /// <summary>
        /// Starts a new soft transaction to ensure multiple operations are saved all at once.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the current transaction, saving all changes made during the transaction.
        /// </summary>
        Task CommitTransactionAsync();
    }
}