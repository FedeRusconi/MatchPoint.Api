using System.ComponentModel.DataAnnotations;

namespace MatchPoint.Api.Shared.AccessControlService.Enums
{
    public enum RoleCapabilityAction
    {
        None,
        Read,
        [Display(Name = "Read & Write")]
        ReadWrite,
        [Display(Name = "Read, Write & Delete")]
        ReadWriteDelete
    }
}
