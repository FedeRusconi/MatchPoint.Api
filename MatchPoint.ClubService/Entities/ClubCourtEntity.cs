using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.ClubService.Entities
{
    public class ClubCourtEntity : IDeactivable
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        // Add more properties as they are needed

    }
}
