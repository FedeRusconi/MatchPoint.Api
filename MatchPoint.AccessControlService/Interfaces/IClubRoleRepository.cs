﻿using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.AccessControlService.Interfaces
{
    public interface IClubRoleRepository : IRepository<ClubRoleEntity>, IQueryableRepository<ClubRoleEntity>
    {
    }
}
