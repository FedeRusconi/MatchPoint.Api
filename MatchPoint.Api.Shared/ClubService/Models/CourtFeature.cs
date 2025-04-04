namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class CourtFeature
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required object Value { get; set; }
    }
}
