namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class ClubMember
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public string? Photo { get; set; }
        // Add more properties as they are needed
        // Possibly a MembershipType is needed
    }
}
