using System.Text.Json;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data.Factories;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Tests.Unit.Infrastructure.Data.Factories
{
    [TestClass]
    public class AzureAdUserFactoryTests
    {
        private IAzureAdUserFactory _factory = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new AzureAdUserFactory();
        }

        [TestMethod]
        public void PatchedUser_WithValidPropertyUpdates_ShouldReturnedPatchedUser()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string firstName = "Updated First Name";
            string lastName = "Updated Last Name";
            string jobTitle = "Updated Job Title";
            string phone = "Updated Mobile Phone";
            string businessPhone = "Updated Business Phone";
            ActiveStatus status = ActiveStatus.Active;
            Address address = new()
            {
                Street = "Test",
                City = "Test",
                State = "QLD",
                PostalCode = "Test",
                Country = new() { Code = "Test", Name = "Test" }
            };
            DateTime hiredDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5));
            DateTime letftDate = DateTime.UtcNow;
            List<PropertyUpdate> updates = [
                new() { Property = nameof(ClubStaffEntity.FirstName), Value = firstName },
                new() { Property = nameof(ClubStaffEntity.LastName), Value = lastName },
                new() { Property = nameof(ClubStaffEntity.JobTitle), Value = jobTitle },
                new() { Property = nameof(ClubStaffEntity.PhoneNumber), Value = phone },
                new() { Property = nameof(ClubStaffEntity.BusinessPhoneNumber), Value = businessPhone },
                new() { Property = nameof(ClubStaffEntity.ActiveStatus), Value = status },
                new() { Property = nameof(ClubStaffEntity.Address), Value = JsonSerializer.Serialize(address)},
                new() { Property = nameof(ClubStaffEntity.HiredOnUtc), Value = hiredDate},
                new() { Property = nameof(ClubStaffEntity.LeftOnUtc), Value = letftDate}
            ];

            // Act
            var adUser = _factory.PatchedUser(updates, id.ToString());

            // Assert
            Assert.IsNotNull(adUser);
            Assert.AreEqual(id.ToString(), adUser.Id);
            Assert.AreEqual(firstName, adUser.GivenName);
            Assert.AreEqual(lastName, adUser.Surname);
            Assert.AreEqual(jobTitle, adUser.JobTitle);
            Assert.AreEqual(phone, adUser.MobilePhone);
            Assert.IsNotNull(adUser.BusinessPhones);
            Assert.AreEqual(businessPhone, adUser.BusinessPhones.FirstOrDefault());
            Assert.IsTrue(adUser.AccountEnabled);
            Assert.AreEqual(address.Street, adUser.StreetAddress);
            Assert.AreEqual(address.City, adUser.City);
            Assert.AreEqual(address.State, adUser.State);
            Assert.AreEqual(address.PostalCode, adUser.PostalCode);
            Assert.AreEqual(address.Country.Name, adUser.Country);
            Assert.AreEqual(hiredDate.Date, adUser.EmployeeHireDate!.Value.Date);
            Assert.AreEqual(letftDate.Date, adUser.EmployeeLeaveDateTime!.Value.Date);
        }

        [TestMethod]
        public void PatchedUser_WithNullPropertyUpdates_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<PropertyUpdate> updates = null!;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _factory.PatchedUser(updates));
        }
    }
}
