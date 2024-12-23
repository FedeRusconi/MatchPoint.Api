using System.ComponentModel.DataAnnotations.Schema;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;
using MatchPoint.Api.Shared.Common.Models;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Entities
{
    public class ClubStaffEntity : ITrackable, IAuditable, IPatchable
    {
        #region Database Properties

        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public required string Email { get; set; }
        public string? Photo { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }

        #endregion
        #region Azure AD Properties
        
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? JobTitle { get; set; }
        [NotMapped]
        public string? PhoneNumber { get; set; }
        [NotMapped]
        public string? BusinessPhoneNumber { get; set; }
        [NotMapped]
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        [NotMapped]
        public Address? Address { get; set; }
        [NotMapped]
        public Guid? ManagerId { get; set; }
        [NotMapped]
        public DateTime? HiredOnUtc { get; set; }
        [NotMapped]
        public DateTime? LeftOn { get; set; }

        #endregion

        /// <summary>
        /// Set the properties defined in Azure AD.
        /// </summary>
        /// <param name="adUser"> The Azure AD user. </param>
        public void SetAzureAdProperties(User adUser)
        {
            FirstName = adUser.GivenName;
            LastName = adUser.Surname;
            JobTitle = adUser.JobTitle;
            PhoneNumber = adUser.MobilePhone;
            BusinessPhoneNumber = adUser.BusinessPhones?.FirstOrDefault();
            ActiveStatus = adUser.AccountEnabled == true ? ActiveStatus.Active : ActiveStatus.Active;
            Address = new()
            {
                Street = adUser.StreetAddress ?? string.Empty,
                City = adUser.City ?? string.Empty,
                State = adUser.State ?? string.Empty,
                PostalCode = adUser.PostalCode ?? string.Empty,
                Country = new() { Code = string.Empty, Name = adUser.Country ?? string.Empty },
            };
            ManagerId = Guid.TryParse(adUser.Manager?.Id, out var guid) ? guid : null;
            HiredOnUtc = adUser.EmployeeHireDate != null ? ((DateTimeOffset)adUser.EmployeeHireDate).UtcDateTime : null;
            LeftOn = adUser.EmployeeLeaveDateTime != null ? ((DateTimeOffset)adUser.EmployeeLeaveDateTime).UtcDateTime : null;
        }
    }
}
