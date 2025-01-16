using MatchPoint.Api.Shared.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Shared.AccessControlService.Models
{
    public class ClubRole : ICustomRole, ITrackable, IDeactivable
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
