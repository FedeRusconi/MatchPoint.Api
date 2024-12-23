using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Tests.Integration.Services
{
    [TestClass]
    public class AzureAdServiceTests
    {
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private AzureAdService _azureAdService = default!;

        [TestInitialize]
        public void Setup()
        {
            _azureAdService = new AzureAdService(_configuration);
        }

        #region GetUserByIdAsync

        [TestMethod]
        public async Task GetUserByIdAsync_WithValidUserId_ShouldReturnUserFromAzureAd()
        {
            // Arrange
            Guid userId = TestAuthHandler.ObjectIdValue;

            // Act
            var result = await _azureAdService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId.ToString(), result.Id);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            // Act
            var result = await _azureAdService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetUserManagerAsync

        [TestMethod]
        public async Task GetUserManagerAsync_WithValidUserId_ShouldReturnUserManagerFromAzureAd()
        {
            // Arrange
            Guid userId = TestAuthHandler.ObjectIdValue;

            // Act
            var result = await _azureAdService.GetUserManagerAsync(userId);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetUserManagerAsync_WhenUserHasNoManager_ShouldReturnNull()
        {
            // Arrange
            Guid userId = TestAuthHandler.ManagerObjectIdValue;

            // Act
            var result = await _azureAdService.GetUserManagerAsync(userId);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region CreateUserAsync

        [TestMethod]
        public async Task CreateUserAsync_WithValidUser_ShouldCreateAndReturnUserInAzureAd()
        {
            // Arrange
            // TODO - Add a user builder in helpers
            var azureAdUser = new User()
            {
                GivenName = "Integration",
                Surname = "Test",
                DisplayName = "Integration Test",
                UserPrincipalName = $"federusconi_live.it@matchpointdev.onmicrosoft.com",
                MailNickname = "Integration.Test",
                JobTitle = "Tester",
                Identities = [new() {  SignInType = "emailAddress", Issuer= "matchpointdev.onmicrosoft.com", IssuerAssignedId="federusconi@live.it"}],
                PasswordProfile = new() { Password = "Test123465", ForceChangePasswordNextSignIn = false},
                AccountEnabled = true
            };

            // Act
            var result = await _azureAdService.CreateUserAsync(azureAdUser);

            // Assert
            Assert.IsNotNull(result);
            // More assertions here (mail, status)
        }

        #endregion
    }
}
