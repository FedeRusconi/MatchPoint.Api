﻿using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubStaffMapper
    {
        #region To ClubStaffEntity

        /// <summary>
        /// Map a <see cref="ClubStaff"/> instance to a <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="clubStaff"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubStaffEntity"/>. </returns>
        public static ClubStaffEntity ToClubStaffEntity(this ClubStaff clubStaff)
        {
            return new ClubStaffEntity()
            {
                Id = clubStaff.Id,
                ClubId = clubStaff.ClubId,
                Photo = clubStaff.Photo,
                RoleId = clubStaff.RoleId,
                RoleName = clubStaff.RoleName,
                CreatedBy = clubStaff.CreatedBy,
                CreatedOnUtc = clubStaff.CreatedOnUtc,
                ModifiedBy = clubStaff.ModifiedBy,
                ModifiedOnUtc = clubStaff.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubStaff"/> instances to an Enumerable of <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="clubStaffs"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubStaffEntity"/>. </returns>
        public static IEnumerable<ClubStaffEntity> ToClubStaffEntityEnumerable(this IEnumerable<ClubStaff> clubStaffs)
        {
            return clubStaffs.Select(staff => staff.ToClubStaffEntity());
        }

        #endregion
        #region To ClubStaffDto

        /// <summary>
        /// Map a <see cref="ClubStaffEntity"/> instance to a <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="clubStaffEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubStaff"/>. </returns>
        public static ClubStaff ToClubStaffDto(this ClubStaffEntity clubStaffEntity)
        {
            return new ClubStaff()
            {
                // TODO: Add properties from Azure AD
                Id = clubStaffEntity.Id,
                ClubId = clubStaffEntity.ClubId,
                Photo = clubStaffEntity.Photo,
                RoleId = clubStaffEntity.RoleId,
                RoleName = clubStaffEntity.RoleName,
                CreatedBy = clubStaffEntity.CreatedBy,
                CreatedOnUtc = clubStaffEntity.CreatedOnUtc,
                ModifiedBy = clubStaffEntity.ModifiedBy,
                ModifiedOnUtc = clubStaffEntity.ModifiedOnUtc,
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubStaffEntity"/> instances to an Enumerable of <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="clubStaffEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubStaff"/>. </returns>
        public static IEnumerable<ClubStaff> ToClubStaffDtoEnumerable(this IEnumerable<ClubStaffEntity> clubStaffEntities)
        {
            return clubStaffEntities.Select(staffEntity => staffEntity.ToClubStaffDto());
        }

        #endregion
    }
}
