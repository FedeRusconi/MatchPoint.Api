using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface ICourtRepository : IRepository<CourtEntity>, IQueryableRepository<CourtEntity>
    {
    }
}
