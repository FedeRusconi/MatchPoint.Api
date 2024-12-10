using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Tests.Unit.Helpers
{
    internal class TrackableEntityTest : ITrackable
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
