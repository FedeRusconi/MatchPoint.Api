namespace MatchPoint.ClubService.Entities
{
    public class ClubMemberEntity
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Photo { get; set; }
        // Add more properties as they are needed
        // Possibly a MembershipType is needed
    }
}
