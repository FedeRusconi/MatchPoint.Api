namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class ClubStaff
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public string? Photo { get; set; }
        // Add more properties as they are needed
        // Possible a Role is needed for RBAC in club admin app
    }
}
