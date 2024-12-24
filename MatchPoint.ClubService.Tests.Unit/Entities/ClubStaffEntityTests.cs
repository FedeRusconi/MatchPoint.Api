using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Tests.Unit.Entities
{
    [TestClass]
    public class ClubStaffEntityTests
    {
        private ClubStaffEntityBuilder _clubStaffEntityBuilder = default!;
        private AzureAdUserBuilder _azureAdUserBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _clubStaffEntityBuilder = new();
            _azureAdUserBuilder = new();
        }

        [TestMethod]
        public void SetAzureAdProperties_WithValidUser_ShouldSetPropertiesOfClubStaff()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clubStaff = _clubStaffEntityBuilder.WithId(userId).Build();
            var azureAdUser = _azureAdUserBuilder
                .WithId(userId)
                .WithManager()
                .WithAddress()
                .WithPhoneNumbers()
                .WithEmploymentDates()
                .Build();

            // Act
            clubStaff.SetAzureAdProperties(azureAdUser);

            // Assert
            Assert.AreEqual(ActiveStatus.Active, clubStaff.ActiveStatus);
            Assert.AreEqual(azureAdUser.GivenName, clubStaff.FirstName);
            Assert.AreEqual(azureAdUser.Surname, clubStaff.LastName);
            Assert.AreEqual(azureAdUser.JobTitle, clubStaff.JobTitle);
            Assert.AreEqual(azureAdUser.MobilePhone, clubStaff.PhoneNumber);
            Assert.AreEqual(azureAdUser.BusinessPhones?.First(), clubStaff.BusinessPhoneNumber);
            Assert.AreEqual(azureAdUser.Manager?.Id, clubStaff.ManagerId.ToString());
            Assert.AreEqual(azureAdUser.EmployeeHireDate, clubStaff.HiredOnUtc);
            Assert.AreEqual(azureAdUser.EmployeeLeaveDateTime, clubStaff.LeftOnUtc);
            Assert.IsNotNull(clubStaff.Address);
            Assert.AreEqual(azureAdUser.StreetAddress, clubStaff.Address.Street);
            Assert.AreEqual(azureAdUser.City, clubStaff.Address.City);
            Assert.AreEqual(azureAdUser.State, clubStaff.Address.State);
            Assert.AreEqual(azureAdUser.PostalCode, clubStaff.Address.PostalCode);
            Assert.AreEqual(azureAdUser.Country, clubStaff.Address.Country.Name);
        }

        [TestMethod]
        public void SetAzureAdProperties_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange;
            var clubStaff = _clubStaffEntityBuilder.Build();
            User azureAdUser = null!;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => clubStaff.SetAzureAdProperties(azureAdUser));
        }
    }
}
