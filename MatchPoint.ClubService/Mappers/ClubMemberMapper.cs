using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubMemberMapper
    {
        #region To ClubMemberEntity

        /// <summary>
        /// Map a <see cref="ClubMember"/> instance to a <see cref="ClubMemberEntity"/>.
        /// </summary>
        /// <param name="clubMember"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubMemberEntity"/>. </returns>
        public static ClubMemberEntity ToClubMemberEntity(this ClubMember clubMember)
        {
            return new ClubMemberEntity()
            {
                Id = clubMember.Id,
                FirstName = clubMember.FirstName,
                LastName = clubMember.LastName,
                Photo = clubMember.Photo
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubMember"/> instances to an Enumerable of <see cref="ClubMemberEntity"/>.
        /// </summary>
        /// <param name="clubMembers"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubMemberEntity"/>. </returns>
        public static IEnumerable<ClubMemberEntity> ToClubMemberEntityEnumerable(this IEnumerable<ClubMember> clubMembers)
        {
            return clubMembers.Select(member => member.ToClubMemberEntity());
        }

        #endregion
        #region To ClubMemberDto

        /// <summary>
        /// Map a <see cref="ClubMemberEntity"/> instance to a <see cref="ClubMember"/>.
        /// </summary>
        /// <param name="clubMemberEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubMember"/>. </returns>
        public static ClubMember ToClubMemberDto(this ClubMemberEntity clubMemberEntity)
        {
            return new ClubMember()
            {
                Id = clubMemberEntity.Id,
                FirstName = clubMemberEntity.FirstName,
                LastName = clubMemberEntity.LastName,
                Photo = clubMemberEntity.Photo
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubMemberEntity"/> instances to an Enumerable of <see cref="ClubMember"/>.
        /// </summary>
        /// <param name="clubMemberEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubMember"/>. </returns>
        public static IEnumerable<ClubMember> ToClubMemberDtoEnumerable(this IEnumerable<ClubMemberEntity> clubMemberEntities)
        {
            return clubMemberEntities.Select(memberEntity => memberEntity.ToClubMemberDto());
        }

        #endregion
    }
}
