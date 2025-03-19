using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.Api.Shared.Infrastructure.RepositoryBases
{
    public abstract class TransactionableRepositoryBase(DbContext _context) : ITransactionableRepository
    {
        /// <inheritdoc />
        public bool IsTransactionActive { get; set; }

        /// <inheritdoc />
        public void BeginTransaction()
        {
            if (IsTransactionActive)
            {
                throw new InvalidOperationException("Transaction is already active.");
            }

            IsTransactionActive = true;
        }

        /// <inheritdoc />
        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (!IsTransactionActive)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            await _context.SaveChangesAsync(cancellationToken);
            IsTransactionActive = false;
        }
    }
}
