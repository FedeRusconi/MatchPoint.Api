using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.AccessControlService.Infrastructure.Data.Repositories
{
    public class ClubRoleRepository(AccessControlServiceDbContext _context, ILogger<ClubRoleRepository> _logger) : 
        IClubRoleRepository
    {
        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ClubRoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IEnumerable<ClubRoleEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<PagedResponse<ClubRoleEntity>> GetAllWithSpecificationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, Dictionary<string, string>? filters = null, Dictionary<string, SortDirection>? orderBy = null, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ClubRoleEntity?> CreateAsync(ClubRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ClubRoleEntity?> UpdateAsync(ClubRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ClubRoleEntity?> DeleteAsync(ClubRoleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
