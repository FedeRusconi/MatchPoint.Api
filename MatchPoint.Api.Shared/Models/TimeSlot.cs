using MatchPoint.Api.Shared.Enums;

namespace MatchPoint.Api.Shared.Models
{
    public class TimeSlot
    {
        public TimeSlotDay AvailabilityDay { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
        public string TimezoneId { get; set; } = TimeZoneInfo.Utc.Id;
    }
}
