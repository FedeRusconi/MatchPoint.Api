using System.Net;
using System.Net.Http.Json;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.AccessControlService.Tests.Integration.Helpers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Extensions;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.AccessControlService.Tests.Integration.Controllers
{
    [TestClass]
    public class CustomRolesControllerTests
    {
        private static WebApplicationFactory<Program> _factory = default!;
        private static HttpClient _httpClient = default!;
        private static AccessControlServiceDbContext _dbContext = null!;
        private CustomRoleEntityBuilder _entityBuilder = default!;
        private CustomRoleBuilder _dtoBuilder = default!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup()
        {
            _factory.Dispose();
            _dbContext.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new(DataContextHelpers.TestingConfiguration);
            _entityBuilder = new CustomRoleEntityBuilder();
            _dtoBuilder = new CustomRoleBuilder();

            // Calls custom Extension method to set up a test http client for tests
            _httpClient = _factory.GetTestHttpClient();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpClient.Dispose();
        }

        #region GetCustomRolesAsync

        [TestMethod]
        public async Task GetCustomRolesAsync_ValidRequest_ShouldReturnAllRecords()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Read";
            CustomRoleEntity customRoleEntity1 = _entityBuilder.WithName("Custom Role 1").Build();
            _entityBuilder = new();
            CustomRoleEntity customRoleEntity2 = _entityBuilder.WithName("Custom Role 2").Build();
            _entityBuilder = new();
            CustomRoleEntity customRoleEntity3 = _entityBuilder.WithName("Custom Role 3").Build();

            try
            {
                // First create test custom roles
                _dbContext.CustomRoles.Add(customRoleEntity1);
                _dbContext.CustomRoles.Add(customRoleEntity2);
                _dbContext.CustomRoles.Add(customRoleEntity3);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles");
                var response = await result.Content.ReadFromJsonAsync<IEnumerable<CustomRole>>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(response);
                Assert.AreEqual(3, response.Count());
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(customRoleEntity1);
                _dbContext.Remove(customRoleEntity2);
                _dbContext.Remove(customRoleEntity3);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetCustomRolesAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Read";
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task GetCustomRolesAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";

            // Act
            var result = await _httpClient.GetAsync($"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region GetCustomRoleAsync

        [TestMethod]
        public async Task GetCustomRoleAsync_WithValidId_ShouldReturnSingleRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Read";
            Guid customRoleId = Guid.NewGuid();
            CustomRoleEntity customRoleEntity = _entityBuilder
                .WithId(customRoleId)
                .WithName("Custom Role 1")
                .Build();

            try
            {
                // First create test custom role
                _dbContext.CustomRoles.Add(customRoleEntity);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");
                var customRoleResponse = await result.Content.ReadFromJsonAsync<CustomRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(customRoleResponse);
                Assert.AreEqual(customRoleId, customRoleResponse.Id);
                Assert.AreEqual(customRoleEntity.Name, customRoleResponse.Name);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(customRoleEntity);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetCustomRoleAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            string customRoleId = "Invalid Id";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetCustomRoleAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Read";
            Guid customRoleId = Guid.NewGuid();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task GetCustomRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid customRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PostCustomRoleAsync

        [TestMethod]
        public async Task PostCustomRoleAsync_WithValidCustomRole_ShouldCreateAndReturnCreatedRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.WithDefaultId().Build();
            CustomRole? customRoleResponse = null;

            try
            {
                // Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles", customRole);
                customRoleResponse = await result.Content.ReadFromJsonAsync<CustomRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                Assert.IsNotNull(customRoleResponse);
                Assert.AreEqual(customRole.Name, customRoleResponse.Name);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.CustomRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == customRoleResponse.Id);
                Assert.IsNotNull(getResponse);
            }
            finally
            {
                // Cleanup
                if (customRoleResponse != null)
                {
                    _dbContext.Remove(customRoleResponse.ToCustomRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.Build();
            customRole.Name = null!;

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_WithInvalidCustomRole_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            string customRole = "Invalid Custom Role";

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.WithDefaultId().Build();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            CustomRole customRole = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PutCustomRoleAsync

        [TestMethod]
        public async Task PutCustomRoleAsync_WithValidCustomRole_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.Build();
            CustomRole? customRoleResponse = null;
            string updatedName = "Updated Role Name";
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                customRole.Name = updatedName;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", customRole);
                customRoleResponse = await result.Content.ReadFromJsonAsync<CustomRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(customRoleResponse);
                Assert.AreEqual(customRole.Id, customRoleResponse.Id);
                Assert.AreEqual(updatedName, customRoleResponse.Name);
                Assert.AreNotEqual(customRole.ModifiedOnUtc, customRoleResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.CustomRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == customRoleResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedName, getResponse.Name);
                Assert.AreNotEqual(customRole.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (customRoleResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(customRoleResponse.ToCustomRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            string invalidCustomRoleId = "Invalid CustomRole Id";
            CustomRole customRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{invalidCustomRoleId}", customRole);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                customRole.Name = null!;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", customRole);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.WithDefaultId().Build();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            CustomRole customRole = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", customRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PatchCustomRoleAsync

        [TestMethod]
        public async Task PatchCustomRoleAsync_WithValidCustomRole_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.WithActiveStatus(ActiveStatus.Active).Build();
            CustomRole? customRoleResponse = null;
            string updatedName = "New Custom Role Name";
            ActiveStatus updatedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(CustomRole.ActiveStatus), Value = updatedStatus },
                new() { Property = nameof(CustomRole.Name), Value = updatedName },
            ];
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", updates);
                customRoleResponse = await result.Content.ReadFromJsonAsync<CustomRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(customRoleResponse);
                Assert.AreEqual(customRole.Id, customRoleResponse.Id);
                Assert.AreEqual(updatedName, customRoleResponse.Name);
                Assert.AreEqual(updatedStatus, customRoleResponse.ActiveStatus);
                Assert.AreNotEqual(customRole.ModifiedOnUtc, customRoleResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.CustomRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == customRoleResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedStatus, getResponse.ActiveStatus);
                Assert.AreEqual(updatedName, getResponse.Name);
                Assert.AreNotEqual(customRole.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (customRoleResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(customRoleResponse.ToCustomRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            string invalidClubId = "Invalid Custom Role Id";
            CustomRole customRole = _dtoBuilder.Build();
            string updatedName = "New Custom Role Name";
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(CustomRole.Name), Value = updatedName }
            ];
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{invalidClubId}", updates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_WithInvalidUpdates_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            CustomRole customRole = _dtoBuilder.Build();
            string invalidPropertyUpdates = "Invalid Property Updates";
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}", invalidPropertyUpdates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Write";
            Guid customRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(CustomRole.Name), Value = "Updated Name" }
            ];
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}", updates);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid customRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(CustomRole.Name), Value = "Updated Name" }
            ];

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}", updates);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region DeleteCustomRoleAsync

        [TestMethod]
        public async Task DeleteCustomRoleAsync_WithValidId_ShouldDeleteRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Delete";
            CustomRole customRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRole.Id}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

                // Ensure record is actually deleted in DB
                var getResponse = await _dbContext.CustomRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == customRole.Id);
                Assert.IsNull(getResponse);
            }
            catch (Exception)
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Delete";
            string invalidCustomRoleId = "Invalid CustomRole Id";
            CustomRole customRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.CustomRoles.Add(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{invalidCustomRoleId}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(customRole.ToCustomRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Delete";
            Guid customRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "CustomRoles.Delete";
            Guid customRoleId = Guid.NewGuid();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(authenticated: false);

            // Act
            var result = await _httpClient.DeleteAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid customRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/customRoles/{customRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion
    }
}
