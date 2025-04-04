using System.ComponentModel.DataAnnotations;

namespace MatchPoint.Api.Shared.ClubService.Enums
{
    // Lookup Table instead?
    public enum CourtRatingAttribute
    {
        Condition,
        Speed,
        Bounce,
        Traction,
        Comfort,
        Durability,
        [Display(Name = "Weather Resistance")]
        WeatherResistance
    }
}
