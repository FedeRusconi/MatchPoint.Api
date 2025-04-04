using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class CourtMapper
    {
        #region To CourtEntity

        /// <summary>
        /// Map a <see cref="Court"/> instance to a <see cref="ClubCourtEntity"/>.
        /// </summary>
        /// <param name="court"> The instance to convert. </param>
        /// <returns> An instance of <see cref="ClubCourtEntity"/>. </returns>
        public static ClubCourtEntity ToCourtEntity(this Court court)
        {
            return new ClubCourtEntity()
            {
                Id = court.Id,
                Name = court.Name,
                ActiveStatus = court.ActiveStatus
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="Court"/> instances to an Enumerable of <see cref="ClubCourtEntity"/>.
        /// </summary>
        /// <param name="courts"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="ClubCourtEntity"/>. </returns>
        public static IEnumerable<ClubCourtEntity> ToCourtEntityEnumerable(this IEnumerable<Court> courts)
        {
            return courts.Select(court => court.ToCourtEntity());
        }

        #endregion
        #region To CourtDto

        /// <summary>
        /// Map a <see cref="ClubCourtEntity"/> instance to a <see cref="Court"/>.
        /// </summary>
        /// <param name="courtEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="Court"/>. </returns>
        public static Court ToCourtDto(this ClubCourtEntity courtEntity)
        {
            return new Court()
            {
                Id = courtEntity.Id,
                Name = courtEntity.Name,
                ActiveStatus = courtEntity.ActiveStatus
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="ClubCourtEntity"/> instances to an Enumerable of <see cref="Court"/>.
        /// </summary>
        /// <param name="courtEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="Court"/>. </returns>
        public static IEnumerable<Court> ToCourtDtoEnumerable(this IEnumerable<ClubCourtEntity> courtEntities)
        {
            return courtEntities.Select(courtEntity => courtEntity.ToCourtDto());
        }

        #endregion
    }
}
