namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class HistoricalCourtMaintenance : CourtMaintenance
    {
        public Guid Id { get; set; }
        public Guid CourtId { get; set; }
        public Surface? Surface { get; set; }
    }
}
