using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubMapper
    {
        #region To ClubEntity

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
                SocialMedia = club.SocialMedia,
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

        #endregion
        #region To ClubDto

        /// <summary>
        /// Map a <see cref="ClubEntity"/> instance to a <see cref="Club"/>.
        /// </summary>
        /// <param name="clubEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="Club"/>. </returns>
        public static Club ToClubDto(this ClubEntity clubEntity)
        {
            return new Club()
            {
                Id = clubEntity.Id,
                Email = clubEntity.Email,
                Name = clubEntity.Name,
                PhoneNumber = clubEntity.PhoneNumber,
                Address = clubEntity.Address,
                Logo = clubEntity.Logo,
                ActiveStatus = clubEntity.ActiveStatus,
                TaxId = clubEntity.TaxId,
                OpeningTimes = clubEntity.OpeningTimes,
                SocialMedia = clubEntity.SocialMedia,
                Members = clubEntity.Members.ToClubMemberDtoEnumerable().ToList(),
                CreatedBy = clubEntity.CreatedBy,
                CreatedOnUtc = clubEntity.CreatedOnUtc,
                ModifiedBy = clubEntity.ModifiedBy,
                ModifiedOnUtc = clubEntity.ModifiedOnUtc,
                TimezoneId = clubEntity.TimezoneId
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubEntity"/> instances to an Enumerable of <see cref="Club"/>.
        /// </summary>
        /// <param name="clubEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="Club"/>. </returns>
        public static IEnumerable<Club> ToClubDtoEnumerable(this IEnumerable<ClubEntity> clubEntities)
        {
            return clubEntities.Select(clubEntity => clubEntity.ToClubDto());
        }

        #endregion
    }
}
