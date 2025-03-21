﻿using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.AccessControlService.Entities
{
    /// <summary>
    /// This entity represents pre-defined roles that clubs can implement.
    /// </summary>
    public class CustomRoleEntity : ICustomRoleEntity, IDeactivable, ITrackable, IPatchable
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; } = [];
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
