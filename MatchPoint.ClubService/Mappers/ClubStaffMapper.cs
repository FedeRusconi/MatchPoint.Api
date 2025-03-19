using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.ClubService.Entities;
using Microsoft.Graph.Models;

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
                Email = clubStaff.Email,
                FirstName = clubStaff.FirstName,
                LastName = clubStaff.LastName,
                Photo = clubStaff.Photo,
                RoleId = clubStaff.RoleId,
                JobTitle = clubStaff.JobTitle,
                PhoneNumber = clubStaff.PhoneNumber,
                BusinessPhoneNumber = clubStaff.BusinessPhoneNumber,
                ActiveStatus = clubStaff.ActiveStatus,
                Address = clubStaff.Address,
                ManagerId = clubStaff.ManagerId,
                HiredOnUtc = clubStaff.HiredOnUtc,
                LeftOnUtc = clubStaff.LeftOnUtc,
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
                Id = clubStaffEntity.Id,
                ClubId = clubStaffEntity.ClubId,
                Email = clubStaffEntity.Email,
                FirstName = clubStaffEntity.FirstName ?? string.Empty,
                LastName = clubStaffEntity.LastName ?? string.Empty,
                JobTitle = clubStaffEntity.JobTitle,
                PhoneNumber = clubStaffEntity.PhoneNumber,
                BusinessPhoneNumber = clubStaffEntity.BusinessPhoneNumber,
                Photo = clubStaffEntity.Photo,
                RoleId = clubStaffEntity.RoleId,
                ActiveStatus = clubStaffEntity.ActiveStatus,
                Address = clubStaffEntity.Address,
                ManagerId = clubStaffEntity.ManagerId,
                HiredOnUtc = clubStaffEntity.HiredOnUtc,
                LeftOnUtc = clubStaffEntity.LeftOnUtc,
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
        #region To AzureAdUser

        /// <summary>
        /// Map a <see cref="ClubStaffEntity"/> instance to a AzureAD <see cref="User"/>.
        /// </summary>
        /// <param name="clubStaffEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="User"/>. </returns>
        public static User ToAzureAdUser(this ClubStaffEntity clubStaffEntity)
        {
            return new User()
            {
                Id = clubStaffEntity.Id.ToString(),
                GivenName = clubStaffEntity.FirstName,
                Surname = clubStaffEntity.LastName,
                JobTitle = clubStaffEntity.JobTitle,
                MobilePhone = clubStaffEntity.PhoneNumber,
                BusinessPhones = [clubStaffEntity.BusinessPhoneNumber],
                AccountEnabled = clubStaffEntity.IsActive(),
                StreetAddress = clubStaffEntity.Address?.Street,
                City = clubStaffEntity.Address?.City,
                State = clubStaffEntity.Address?.State,
                PostalCode = clubStaffEntity.Address?.PostalCode,
                Country = clubStaffEntity.Address?.Country.Name,
                EmployeeHireDate = clubStaffEntity.HiredOnUtc,
                EmployeeLeaveDateTime = clubStaffEntity.LeftOnUtc,
            };
        }

        #endregion
    }
}
