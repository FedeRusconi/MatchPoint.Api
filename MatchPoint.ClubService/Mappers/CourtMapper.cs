using MatchPoint.Api.Shared.CourtService.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.ClubService.Mappers
{
    public static class CourtMapper
    {
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
    }
}
