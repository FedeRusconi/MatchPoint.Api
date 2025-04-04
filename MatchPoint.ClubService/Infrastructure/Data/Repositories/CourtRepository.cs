using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class CourtRepository(ClubServiceDbContext _context, ILogger<CourtRepository> _logger) : ICourtRepository
    {
        public Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            throw new NotImplementedException();
        }

        public Task<CourtEntity?> CreateAsync(CourtEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CourtEntity?> DeleteAsync(CourtEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourtEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<CourtEntity>> GetAllWithSpecificationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, Dictionary<string, string>? filters = null, Dictionary<string, SortDirection>? orderBy = null, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        public Task<CourtEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        public Task<CourtEntity?> UpdateAsync(CourtEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
