using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.ClubService.Entities
{
    /// <summary>
    /// This is used for historical maintenance
    /// </summary>
    public class CourtMaintenanceEntity
    {
        public Guid Id { get; set; }
        public Guid CourtId { get; set; }
        public Surface? Surface { get; set; }
        public DateTime NextMaintenance { get; set; }
        public Frequency? Frequency { get; set; }
        public string? Description { get; set; }
        public Frequency? ReminderFrequency { get; set; }
        public DateTime LastMaintenance { get; set; }
    }
}
