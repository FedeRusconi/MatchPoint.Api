namespace MatchPoint.Api.Tests.Unit.Helpers
{
    internal class GenericEntityTest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
