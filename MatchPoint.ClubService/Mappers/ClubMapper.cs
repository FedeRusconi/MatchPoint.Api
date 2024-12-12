using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubMapper
    {
        /// <summary>
        /// Map a <see cref="Club"/> instance to a <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="club"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubEntity"/>. </returns>
        public static ClubEntity ToClubEntity(this Club club)
        {
            return new ClubEntity()
            {
                Id = club.Id,
                Email = club.Email,
                Name = club.Name,
                PhoneNumber = club.PhoneNumber,
                Address = club.Address,
                Logo = club.Logo,
                ActiveStatus = club.ActiveStatus,
                TaxId = club.TaxId,
                OpeningTimes = club.OpeningTimes,
                Courts = club.Courts.ToCourtEntityEnumerable().ToList(),
                SocialMedia = club.SocialMedia,
                Staff = club.Staff.ToClubMemberEntityEnumerable().ToList(),
                Members = club.Members.ToClubMemberEntityEnumerable().ToList(),
                CreatedBy = club.CreatedBy,
                CreatedOnUtc = club.CreatedOnUtc,
                ModifiedBy = club.ModifiedBy,
                ModifiedOnUtc = club.ModifiedOnUtc,
                TimezoneId = club.TimezoneId
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="Club"/> instances to an Enumerable of <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="clubs"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubEntity"/>. </returns>
        public static IEnumerable<ClubEntity> ToClubEntityEnumerable(this IEnumerable<Club> clubs)
        {
            return clubs.Select(club => club.ToClubEntity());
        }
    }
}
