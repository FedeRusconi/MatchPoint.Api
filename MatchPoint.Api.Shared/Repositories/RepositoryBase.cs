using MatchPoint.Api.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.Api.Shared.Repositories
{
    public abstract class RepositoryBase(DbContext _context) : IRepository
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
        public async Task CommitTransactionAsync()
        {
            if (!IsTransactionActive)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            await _context.SaveChangesAsync();
            IsTransactionActive = false;
        }
    }
}
