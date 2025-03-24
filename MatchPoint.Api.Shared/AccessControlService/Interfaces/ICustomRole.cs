using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.Api.Shared.AccessControlService.Interfaces
{
    public interface ICustomRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; }
    }
}
