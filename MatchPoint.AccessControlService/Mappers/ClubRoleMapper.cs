using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.AccessControlService.Mappers
{
    public static class ClubRoleMapper
    {
        #region ToClubRoleEntity

        /// <summary>
        /// Map a <see cref="ClubRole"/> instance to a <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="clubRole"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubRoleEntity"/>. </returns>
        public static ClubRoleEntity ToClubRoleEntity(this ClubRole clubRole)
        {
            return new ClubRoleEntity()
            {
                Id = clubRole.Id,
                ClubId = clubRole.ClubId,
                Name = clubRole.Name,
                Capabilities = clubRole.Capabilities,
                ActiveStatus = clubRole.ActiveStatus,
                CreatedBy = clubRole.CreatedBy,
                CreatedOnUtc = clubRole.CreatedOnUtc,
                ModifiedBy = clubRole.ModifiedBy,
                ModifiedOnUtc = clubRole.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubRole"/> instances to an Enumerable of <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="clubRoles"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubRoleEntity"/>. </returns>
        public static IEnumerable<ClubRoleEntity> ToClubRoleEntityEnumerable(this IEnumerable<ClubRole> clubRoles)
        {
            return clubRoles.Select(clubRole => clubRole.ToClubRoleEntity());
        }

        #endregion
        #region ToClubRoleDto

        /// <summary>
        /// Map a <see cref="ClubRoleEntity"/> instance to a <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="clubRoleEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubRole"/>. </returns>
        public static ClubRole ToClubRoleDto(this ClubRoleEntity clubRoleEntity)
        {
            return new ClubRole()
            {
                Id = clubRoleEntity.Id,
                ClubId = clubRoleEntity.ClubId,
                Name = clubRoleEntity.Name,
                Capabilities = clubRoleEntity.Capabilities,
                ActiveStatus = clubRoleEntity.ActiveStatus,
                CreatedBy = clubRoleEntity.CreatedBy,
                CreatedOnUtc = clubRoleEntity.CreatedOnUtc,
                ModifiedBy = clubRoleEntity.ModifiedBy,
                ModifiedOnUtc = clubRoleEntity.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubRoleEntity"/> instances to an Enumerable of <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="clubRoleEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubRole"/>. </returns>
        public static IEnumerable<ClubRole> ToClubRoleDtoEnumerable(this IEnumerable<ClubRoleEntity> clubRoleEntities)
        {
            return clubRoleEntities.Select(entity => entity.ToClubRoleDto());
        }

        #endregion
    }
}
