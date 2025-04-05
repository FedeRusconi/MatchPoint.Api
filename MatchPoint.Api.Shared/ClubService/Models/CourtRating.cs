using MatchPoint.Api.Shared.ClubService.Enums;

namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class CourtRating
    {
        public CourtRatingAttribute Attribute { get; set; }
        public int Value { get; set; }
    }
}
