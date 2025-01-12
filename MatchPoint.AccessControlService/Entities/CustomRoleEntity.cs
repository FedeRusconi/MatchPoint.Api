using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.AccessControlService.Entities
{
    /// <summary>
    /// This entity represents pre-defined roles that clubs can implement.
    /// </summary>
    public class CustomRoleEntity : ICustomRoleEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
    }
}
