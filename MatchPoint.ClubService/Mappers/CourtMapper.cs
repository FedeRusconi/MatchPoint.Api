using MatchPoint.Api.Shared.CourtService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class CourtMapper
    {
        #region To CourtEntity

        /// <summary>
        /// Map a <see cref="Court"/> instance to a <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="court"> The instance to convert. </param>
        /// <returns> An instance of <see cref="CourtEntity"/>. </returns>
        public static CourtEntity ToCourtEntity(this Court court)
        {
            return new CourtEntity()
            {
                Id = court.Id,
                Name = court.Name,
                ActiveStatus = court.ActiveStatus
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="Court"/> instances to an Enumerable of <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="courts"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="CourtEntity"/>. </returns>
        public static IEnumerable<CourtEntity> ToCourtEntityEnumerable(this IEnumerable<Court> courts)
        {
            return courts.Select(court => court.ToCourtEntity());
        }

        #endregion
        #region To CourtDto

        /// <summary>
        /// Map a <see cref="CourtEntity"/> instance to a <see cref="Court"/>.
        /// </summary>
        /// <param name="courtEntity"> The instance to convert. </param>
        /// <returns> An instance of <see cref="Court"/>. </returns>
        public static Court ToCourtDto(this CourtEntity courtEntity)
        {
            return new Court()
            {
                Id = courtEntity.Id,
                Name = courtEntity.Name,
                ActiveStatus = courtEntity.ActiveStatus
            };
        }

        /// <summary>
        /// Map an Enumerable of <see cref="CourtEntity"/> instances to an Enumerable of <see cref="Court"/>.
        /// </summary>
        /// <param name="courtEntities"> The instances to convert. </param>
        /// <returns> An Enumerable of <see cref="Court"/>. </returns>
        public static IEnumerable<Court> ToCourtDtoEnumerable(this IEnumerable<CourtEntity> courtEntities)
        {
            return courtEntities.Select(courtEntity => courtEntity.ToCourtDto());
        }

        #endregion
    }
}
