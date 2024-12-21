using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IClubStaffRepository : IRepository<ClubStaffEntity>, IQueryableRepository<ClubStaffEntity>
    {
    }
}
