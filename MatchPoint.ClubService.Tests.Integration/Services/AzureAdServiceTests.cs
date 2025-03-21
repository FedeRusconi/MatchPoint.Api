﻿using System.Data;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Tests.Integration.Services
{
    [TestClass]
    public class AzureAdServiceTests
    {
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private AzureAdService _azureAdService = default!;

        private AzureAdUserBuilder _azureAdUserBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _azureAdService = new AzureAdService(_configuration);
            _azureAdUserBuilder = new();
            _cancellationToken = new CancellationToken();
        }

        #region GetUserByIdAsync

        [TestMethod]
        public async Task GetUserByIdAsync_WithValidUserId_ShouldReturnUserFromAzureAd()
        {
            // Arrange
            Guid userId = TestAuthHandler.ObjectIdValue;

            // Act
            var result = await _azureAdService.GetUserByIdAsync(userId, _cancellationToken);

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
            var result = await _azureAdService.GetUserByIdAsync(userId, _cancellationToken);

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
            var result = await _azureAdService.GetUserManagerAsync(userId, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetUserManagerAsync_WhenUserHasNoManager_ShouldReturnNull()
        {
            // Arrange
            Guid userId = TestAuthHandler.ManagerObjectIdValue;

            // Act
            var result = await _azureAdService.GetUserManagerAsync(userId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region CreateUserAsync

        [TestMethod]
        public async Task CreateUserAsync_WithValidUser_ShouldCreateAndReturnUserInAzureAd()
        {
            // Arrange
            var azureAdUser = _azureAdUserBuilder.WithDefaultId().Build();
            User? result = null;

            try
            {
                // Act
                result = await _azureAdService.CreateUserAsync(azureAdUser, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _azureAdService.DeleteUserAsync(Guid.Parse(result.Id!), _cancellationToken);
                }
            }
        }

        #endregion

        #region UpdateUserAsync

        [TestMethod]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateAndReturnUserInAzureAd()
        {
            // Arrange
            var azureAdUser = _azureAdUserBuilder.WithDefaultId().Build();
            User? result = null;
            string updatedJobTitle = "Updated Job Title";
            string updatedCity = "Updated City";
            bool updatedAccountEnabled = false;
            string updatedMobile = "Updated Mobile Phone";
            DateTimeOffset updatedEmpLeaveDate = DateTime.UtcNow;

            try
            {
                result = await _azureAdService.CreateUserAsync(azureAdUser, _cancellationToken);
                // Change properties
                var updatedUser = new User()
                {
                    Id = result!.Id,
                    JobTitle = updatedJobTitle,
                    City = updatedCity,
                    AccountEnabled = updatedAccountEnabled,
                    MobilePhone = updatedMobile,
                    EmployeeLeaveDateTime = updatedEmpLeaveDate
                };

                // Act
                result = await _azureAdService.UpdateUserAsync(updatedUser, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(default, result.Id);
                // Check updates actually happened in Azure AD
                var getResult = await _azureAdService.GetUserByIdAsync(Guid.Parse(result.Id!), _cancellationToken);
                Assert.IsNotNull(getResult);
                Assert.AreEqual(updatedJobTitle, getResult.JobTitle);
                Assert.AreEqual(updatedCity, getResult.City);
                Assert.AreEqual(updatedAccountEnabled, getResult.AccountEnabled);
                Assert.AreEqual(updatedMobile, getResult.MobilePhone);
                Assert.AreEqual(updatedEmpLeaveDate.Date, getResult.EmployeeLeaveDateTime!.Value.Date);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _azureAdService.DeleteUserAsync(Guid.Parse(result.Id!), _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var azureAdUser = _azureAdUserBuilder.WithId(userId).Build();

            // Act
            var result = await _azureAdService.UpdateUserAsync(azureAdUser, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region AssignUserManagerAsync

        [TestMethod]
        public async Task AssignUserManagerAsync_WithValidUserAndManager_ShouldUpdateAndReturnUserInAzureAd()
        {
            // Arrange
            var azureAdUser = _azureAdUserBuilder.WithDefaultId().Build();
            var managerId = TestAuthHandler.ManagerObjectIdValue;

            try
            {
                azureAdUser = await _azureAdService.CreateUserAsync(azureAdUser, _cancellationToken);

                // Act
                var result = await _azureAdService.AssignUserManagerAsync(Guid.Parse(azureAdUser?.Id!), managerId, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(managerId, result);
                // Check updates actually happened in Azure AD
                var getResult = await _azureAdService.GetUserManagerAsync(Guid.Parse(azureAdUser?.Id!), _cancellationToken);
                Assert.IsNotNull(getResult);
                Assert.AreEqual(managerId.ToString(), getResult.Id);
            }
            finally
            {
                // Cleanup
                await _azureAdService.DeleteUserAsync(Guid.Parse(azureAdUser?.Id!), _cancellationToken);
            }
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public async Task AssignUserManagerAsync_WhenUserOrManagerDoesNotExist_ShouldReturnNull(
            bool userIdNull, bool managerIdNull)
        {
            // Arrange
            var userId = userIdNull ? Guid.NewGuid() : TestAuthHandler.ObjectIdValue;
            var managerId = managerIdNull ? Guid.NewGuid() : TestAuthHandler.ManagerObjectIdValue;

            // Act
            var result = await _azureAdService.AssignUserManagerAsync(userId, managerId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region DeleteUserAsync

        [TestMethod]
        public async Task DeleteUserAsync_WithValidUserId_ShouldDeleteUserInAzureAd()
        {
            // Arrange
            var azureAdUser = _azureAdUserBuilder.WithDefaultId().Build();
            User? createResult = await _azureAdService.CreateUserAsync(azureAdUser, _cancellationToken);

            // Act
            var result = await _azureAdService.DeleteUserAsync(Guid.Parse(createResult?.Id!), _cancellationToken);

            // Assert
            Assert.IsTrue(result);
            // Ensure user is not in AD anymore
            var getResult = await _azureAdService.GetUserByIdAsync(Guid.Parse(createResult?.Id!), _cancellationToken);
            Assert.IsNull(getResult);
        }

        [TestMethod]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            // Act
            var result = await _azureAdService.DeleteUserAsync(userId, _cancellationToken);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region RemoveUserManagerAsync

        [TestMethod]
        public async Task RemoveUserManagerAsync_WithValidUserAndManager_ShouldRemoveManagerAndReturnTrue()
        {
            // Arrange
            var azureAdUser = _azureAdUserBuilder.WithDefaultId().Build();
            var managerId = TestAuthHandler.ManagerObjectIdValue;

            try
            {
                azureAdUser = await _azureAdService.CreateUserAsync(azureAdUser, _cancellationToken);
                await _azureAdService.AssignUserManagerAsync(Guid.Parse(azureAdUser?.Id!), managerId, _cancellationToken);

                // Act
                var result = await _azureAdService.RemoveUserManagerAsync(Guid.Parse(azureAdUser?.Id!), _cancellationToken);

                // Assert
                Assert.IsTrue(result);
                // Check updates actually happened in Azure AD
                var getResult = await _azureAdService.GetUserManagerAsync(Guid.Parse(azureAdUser?.Id!), _cancellationToken);
                Assert.IsNull(getResult);
            }
            finally
            {
                // Cleanup
                await _azureAdService.DeleteUserAsync(Guid.Parse(azureAdUser?.Id!), _cancellationToken);
            }
        }

        [TestMethod]
        public async Task RemoveUserManagerAsync_WhenUserDoesNotHaveManager_ShouldReturnTrue()
        {
            // Arrange - This user does not have a manager
            var userId = TestAuthHandler.ManagerObjectIdValue;

            // Act
            var result = await _azureAdService.RemoveUserManagerAsync(userId, _cancellationToken);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RemoveUserManagerAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _azureAdService.RemoveUserManagerAsync(userId, _cancellationToken);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
