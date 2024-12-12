using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubStaffMapper
    {
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
                FullName = clubStaff.FullName,
                Photo = clubStaff.Photo
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubStaff"/> instances to an Enumerable of <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="clubStaffs"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubStaffEntity"/>. </returns>
        public static IEnumerable<ClubStaffEntity> ToClubMemberEntityEnumerable(this IEnumerable<ClubStaff> clubStaffs)
        {
            return clubStaffs.Select(staff => staff.ToClubStaffEntity());
        }
    }
}
