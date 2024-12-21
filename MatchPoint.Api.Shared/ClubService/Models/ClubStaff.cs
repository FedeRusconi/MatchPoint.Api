using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class ClubStaff : ITrackable, IAuditable, IPatchable, IDeactivable
    {
        // TODO: Add properties from Azure AD (name, email, phone, etc.)
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public string? Photo { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Address? Address { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
