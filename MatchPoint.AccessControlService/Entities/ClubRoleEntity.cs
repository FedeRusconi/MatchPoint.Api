using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.AccessControlService.Entities
{
    /// <summary>
    /// This entity represents club-specific custom roles.
    /// </summary>
    public class ClubRoleEntity : ICustomRole
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
    }
}
