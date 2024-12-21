using MatchPoint.Api.Shared.ClubService.Enums;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.ClubService.Entities
{
    public class CourtEntity : IDeactivable
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Surface? Surface { get; set; }
        public CourtMaintenance? CourtMaintenance { get; set; }
        public Dictionary<CourtRatingAttribute, int>? Rating {  get; set; }
        // Add: availability, etc.
    }
}
