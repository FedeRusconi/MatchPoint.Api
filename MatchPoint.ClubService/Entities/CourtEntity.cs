using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.ClubService.Entities
{
    public class CourtEntity : IDeactivable, ITrackable, IPatchable
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Surface? Surface { get; set; }
        public CourtMaintenance? CourtMaintenance { get; set; }
        public List<CourtRating>? Ratings { get; set; }
        public List<CourtFeature>? Features { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        // Add: availability, etc.
    }
}
