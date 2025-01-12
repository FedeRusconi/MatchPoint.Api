using MatchPoint.Api.Shared.AccessControlService.Interfaces;

namespace MatchPoint.Api.Shared.AccessControlService.Models
{
    public class CustomRole : ICustomRole
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
    }
}
