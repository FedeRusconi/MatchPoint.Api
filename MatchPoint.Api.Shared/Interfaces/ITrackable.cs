namespace MatchPoint.Api.Shared.Interfaces
{
    public interface ITrackable
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
