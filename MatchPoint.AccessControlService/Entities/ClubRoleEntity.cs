using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.AccessControlService.Entities
{
    /// <summary>
    /// This entity represents club-specific custom roles.
    /// </summary>
    public class ClubRoleEntity : ICustomRoleEntity, IDeactivable, ITrackable, IAuditable, IPatchable
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
