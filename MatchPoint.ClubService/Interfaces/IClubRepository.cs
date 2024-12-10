using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubRepository : 
        IRepository<ClubEntity>, 
        IQueryableRepository<ClubEntity>, 
        ITransactionableRepository
    {
    }
}
