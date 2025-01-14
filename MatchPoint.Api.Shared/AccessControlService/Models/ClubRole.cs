using MatchPoint.Api.Shared.AccessControlService.Interfaces;

namespace MatchPoint.Api.Shared.AccessControlService.Models
{
    public class ClubRole : ICustomRole
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
    }
}
