using MatchPoint.Api.Shared.Interfaces;
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
