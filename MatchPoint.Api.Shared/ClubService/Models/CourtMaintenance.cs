using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class CourtMaintenance
    {
        public DateTime NextMaintenance { get; set; }
        public Frequency? Frequency { get; set; }
        public string? Description { get; set; }
        public Frequency? ReminderFrequency { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
    }
}
