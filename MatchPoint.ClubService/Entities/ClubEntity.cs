using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.ClubService.Entities
{
    public class ClubEntity : ITrackable, IAuditable, IAddressable, IDeactivable, IPatchable
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public required Address Address { get; set; }
        public string? Logo { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public string? TaxId { get; set; }
        public List<TimeSlot> OpeningTimes { get; set; } = [];
        public List<SocialMediaLink> SocialMedia { get; set; } = [];
        public List<ClubMemberEntity> Members { get; set; } = [];
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        public string? TimezoneId { get; set; } = TimeZoneInfo.Utc.Id;
    }
}
