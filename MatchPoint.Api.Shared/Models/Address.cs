namespace MatchPoint.Api.Shared.Models
{
    public class Address
    {
        public required string Street {  get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required Country Country { get; set; }

    }
}
