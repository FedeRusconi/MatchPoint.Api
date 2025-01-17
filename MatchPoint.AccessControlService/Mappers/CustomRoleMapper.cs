using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.AccessControlService.Mappers
{
    public static class CustomRoleMapper
    {
        #region ToCustomRoleEntity

        /// <summary>
        /// Map a <see cref="CustomRole"/> instance to a <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <param name="customRole"> The instance to convert. </param>
        /// <returns> An instance of <see cref="CustomRoleEntity"/>. </returns>
        public static CustomRoleEntity ToCustomRoleEntity(this CustomRole customRole)
        {
            return new CustomRoleEntity()
            {
                Id = customRole.Id,
                Name = customRole.Name,
                Capabilities = customRole.Capabilities,
                ActiveStatus = customRole.ActiveStatus,
                CreatedBy = customRole.CreatedBy,
                CreatedOnUtc = customRole.CreatedOnUtc,
                ModifiedBy = customRole.ModifiedBy,
                ModifiedOnUtc = customRole.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="CustomRole"/> instances to an Enumerable of <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <param name="customRoles"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="CustomRoleEntity"/>. </returns>
        public static IEnumerable<CustomRoleEntity> ToCustomRoleEntityEnumerable(this IEnumerable<CustomRole> customRoles)
        {
            return customRoles.Select(customRole => customRole.ToCustomRoleEntity());
        }

        #endregion
        #region ToCustomRoleDto

        /// <summary>
        /// Map a <see cref="CustomRoleEntity"/> instance to a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="customRoleEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="CustomRole"/>. </returns>
        public static CustomRole ToCustomRoleDto(this CustomRoleEntity customRoleEntity)
        {
            return new CustomRole()
            {
                Id = customRoleEntity.Id,
                Name = customRoleEntity.Name,
                Capabilities = customRoleEntity.Capabilities,
                ActiveStatus = customRoleEntity.ActiveStatus,
                CreatedBy = customRoleEntity.CreatedBy,
                CreatedOnUtc = customRoleEntity.CreatedOnUtc,
                ModifiedBy = customRoleEntity.ModifiedBy,
                ModifiedOnUtc = customRoleEntity.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="CustomRoleEntity"/> instances to an Enumerable of <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="customRoleEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="CustomRole"/>. </returns>
        public static IEnumerable<CustomRole> ToCustomRoleDtoEnumerable(this IEnumerable<CustomRoleEntity> customRoleEntities)
        {
            return customRoleEntities.Select(entity => entity.ToCustomRoleDto());
        }

        #endregion
    }
}
