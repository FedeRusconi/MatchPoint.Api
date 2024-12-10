using MatchPoint.Api.Shared.Common.Enums;

namespace MatchPoint.Api.Shared.Common.Models
{
    public class TimeSlot
    {
        public TimeSlotDay AvailabilityDay { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
        public string TimezoneId { get; set; } = TimeZoneInfo.Utc.Id;
    }
}
