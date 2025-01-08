using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.AccessControlService.Infrastructure.Data.Repositories
{
    public class CustomRoleRepository(AccessControlServiceDbContext _context, ILogger<CustomRoleRepository> _logger) :
        IRepository<CustomRoleEntity>
    {
        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<CustomRoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IEnumerable<CustomRoleEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<CustomRoleEntity?> CreateAsync(CustomRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<CustomRoleEntity?> UpdateAsync(CustomRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<CustomRoleEntity?> DeleteAsync(CustomRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
