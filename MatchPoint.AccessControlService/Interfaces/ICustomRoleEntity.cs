﻿using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.AccessControlService.Interfaces
{
    public interface ICustomRoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<RoleCapability> Capabilities { get; set; }
    }
}
