namespace MatchPoint.Api.Shared.Models
{
    public class PropertyUpdate
    {
        public required string Property { get; set; }
        public object? Value { get; set; }
    }
}
