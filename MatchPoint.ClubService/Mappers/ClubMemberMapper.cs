using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class ClubMemberMapper
    {
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
                FullName = clubMember.FullName,
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
    }
}
