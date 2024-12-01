using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Interfaces;

namespace MatchPoint.ClubService.Entities
{
    public class CourtEntity : IDeactivable
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        // Add more properties as they are needed

    }
}
