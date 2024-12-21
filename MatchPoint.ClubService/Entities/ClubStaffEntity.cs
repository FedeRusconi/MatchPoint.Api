using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.ClubService.Entities
{
    public class ClubStaffEntity : ITrackable, IAuditable, IPatchable
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public string? Photo { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
