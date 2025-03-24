using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.ClubService.Entities
{
    public class ClubStaffEntity : ITrackable, IAuditable, IPatchable, IDeactivable
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Photo { get; set; }
        public Guid? RoleId { get; set; }
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BusinessPhoneNumber { get; set; }
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public Address? Address { get; set; }
        public Guid? ManagerId { get; set; }
        public DateTime? HiredOnUtc { get; set; }
        public DateTime? LeftOnUtc { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
