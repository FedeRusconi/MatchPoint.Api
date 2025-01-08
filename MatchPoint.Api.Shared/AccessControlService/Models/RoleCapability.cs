using MatchPoint.Api.Shared.AccessControlService.Enums;

namespace MatchPoint.Api.Shared.AccessControlService.Models
{
    public class RoleCapability
    {
        public RoleCapabilityAction Action { get; set; }
        public RoleCapabilityFeature Feature { get; set; }
    }
}
