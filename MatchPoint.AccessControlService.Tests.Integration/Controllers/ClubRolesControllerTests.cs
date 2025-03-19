using System.Net;
using System.Net.Http.Json;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.AccessControlService.Tests.Integration.Helpers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MatchPoint.AccessControlService.Tests.Integration.Controllers
{
    [TestClass]
    public class ClubRolesControllerTests
    {
        private static WebApplicationFactory<Program> _factory = default!;
        private static HttpClient _httpClient = default!;
        private static AccessControlServiceDbContext _dbContext = null!;
        private ClubRoleEntityBuilder _entityBuilder = default!;
        private ClubRoleBuilder _dtoBuilder = default!;
        private readonly Guid _clubId = Guid.NewGuid();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Set test authentication
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", options => { });

                    // Replace IConfiguration in the DI container with test-specific configuration
                    services.RemoveAll<IConfiguration>();
                    services.AddSingleton(DataContextHelpers.TestingConfiguration);
                });
            })
            .CreateClient();
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup()
        {
            _httpClient.Dispose();
            _factory.Dispose();
            _dbContext.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new(DataContextHelpers.TestingConfiguration);
            _entityBuilder = new ClubRoleEntityBuilder();
            _dtoBuilder = new ClubRoleBuilder();
        }

        #region GetClubRolesAsync

        [TestMethod]
        public async Task GetClubRolesAsync_WithNoQueryParameters_ShouldReturnAllRecordsWithDefaultPaging()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            ClubRoleEntity clubRoleEntity1 = _entityBuilder
                .WithName("Club Role 1")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubRoleEntity clubRoleEntity2 = _entityBuilder
                .WithName("Club Role 2")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubRoleEntity clubRoleEntity3 = _entityBuilder
                .WithName("Club Role 3")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            // Add a role for a different club id to ensure it is not included.
            ClubRoleEntity clubRoleEntity4 = _entityBuilder
                .WithName("Club Role 4")
                .WithClubId(Guid.NewGuid())
                .Build();

            try
            {
                // First create test club roles
                _dbContext.ClubRoles.Add(clubRoleEntity1);
                _dbContext.ClubRoles.Add(clubRoleEntity2);
                _dbContext.ClubRoles.Add(clubRoleEntity3);
                _dbContext.ClubRoles.Add(clubRoleEntity4);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<ClubRole>>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(1, pagedResponse.CurrentPage);
                Assert.AreEqual(Constants.MaxPageSizeAllowed, pagedResponse.PageSize);
                Assert.AreEqual(3, pagedResponse.TotalCount);
                Assert.AreEqual(3, pagedResponse.Data.Count());
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubRoleEntity1);
                _dbContext.Remove(clubRoleEntity2);
                _dbContext.Remove(clubRoleEntity3);
                _dbContext.Remove(clubRoleEntity4);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubRolesAsync_WithValidQueryParameters_ShouldReturnFilteredSortedRecordsWithPaging()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            int page = 2;
            int pageSize = 1;
            ActiveStatus filterStatus = ActiveStatus.Active;
            ClubRoleEntity clubRoleEntity1 = _entityBuilder
                .WithName("Club Role 1")
                .WithActiveStatus(ActiveStatus.Active)
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubRoleEntity clubRoleEntity2 = _entityBuilder
                .WithName("Club Role 2")
                .WithActiveStatus(ActiveStatus.Active)
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubRoleEntity clubRoleEntity3 = _entityBuilder
                .WithName("Club Role 3")
                .WithActiveStatus(ActiveStatus.Inactive)
                .WithClubId(_clubId)
                .Build();

            try
            {
                // First create test club roles
                _dbContext.ClubRoles.Add(clubRoleEntity2);
                _dbContext.ClubRoles.Add(clubRoleEntity1);
                _dbContext.ClubRoles.Add(clubRoleEntity3);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles" +
                    $"?page={page}&pageSize={pageSize}" +
                    $"&filters[activeStatus]={filterStatus}&orderBy[name]=ascending");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<ClubRole>>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(page, pagedResponse.CurrentPage);
                Assert.AreEqual(pageSize, pagedResponse.PageSize);
                Assert.AreEqual(2, pagedResponse.TotalCount);
                Assert.AreEqual(1, pagedResponse.Data.Count());
                Assert.AreEqual(clubRoleEntity2.Name, pagedResponse.Data.First().Name);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubRoleEntity1);
                _dbContext.Remove(clubRoleEntity2);
                _dbContext.Remove(clubRoleEntity3);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubRolesAsync_WithInvalidQueryParameters_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            string invalidFilters = "Invalid Filters";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles?filters={invalidFilters}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubRolesAsync_WithInvalidClubId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            string clubId = "1";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{clubId}/roles");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubRolesAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region GetClubRoleAsync

        [TestMethod]
        public async Task GetClubRoleAsync_WithValidId_ShouldReturnSingleRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            Guid clubRoleId = Guid.NewGuid();
            ClubRoleEntity clubRoleEntity = _entityBuilder
                .WithId(clubRoleId)
                .WithName("Club Role 1")
                .WithClubId(_clubId)
                .Build();

            try
            {
                // First create test club role
                _dbContext.ClubRoles.Add(clubRoleEntity);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRoleId}");
                var clubRoleResponse = await result.Content.ReadFromJsonAsync<ClubRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubRoleResponse);
                Assert.AreEqual(clubRoleId, clubRoleResponse.Id);
                Assert.AreEqual(clubRoleEntity.Name, clubRoleResponse.Name);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubRoleEntity);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubRoleAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Read";
            string clubRoleId = "Invalid Id";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid clubRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PostClubRoleAsync

        [TestMethod]
        public async Task PostClubRoleAsync_WithValidClubRole_ShouldCreateAndReturnCreatedRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder.WithDefaultId().Build();
            ClubRole? clubRoleResponse = null;

            try
            {
                // Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles", clubRole);
                clubRoleResponse = await result.Content.ReadFromJsonAsync<ClubRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                Assert.IsNotNull(clubRoleResponse);
                Assert.AreEqual(clubRole.Name, clubRoleResponse.Name);
                Assert.AreEqual(_clubId, clubRoleResponse.ClubId);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.ClubRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubRoleResponse.Id);
                Assert.IsNotNull(getResponse);
            }
            finally
            {
                // Cleanup
                if (clubRoleResponse != null)
                {
                    _dbContext.Remove(clubRoleResponse.ToClubRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PostClubRoleAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder.Build();
            clubRole.Name = null!;

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles", clubRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubRoleAsync_WithInvalidClubRole_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            string clubRole = "Invalid Club Role";

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles", clubRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            ClubRole clubRole = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles", clubRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PutClubRoleAsync

        [TestMethod]
        public async Task PutClubRoleAsync_WithValidClubRole_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder.WithClubId(_clubId).Build();
            ClubRole? clubRoleResponse = null;
            string updatedName = "Updated Role Name";
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                clubRole.Name = updatedName;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", clubRole);
                clubRoleResponse = await result.Content.ReadFromJsonAsync<ClubRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubRoleResponse);
                Assert.AreEqual(clubRole.Id, clubRoleResponse.Id);
                Assert.AreEqual(updatedName, clubRoleResponse.Name);
                Assert.AreNotEqual(clubRole.ModifiedOnUtc, clubRoleResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.ClubRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubRoleResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedName, getResponse.Name);
                Assert.AreNotEqual(clubRole.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (clubRoleResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubRoleResponse.ToClubRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PutClubRoleAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            string invalidClubRoleId = "Invalid ClubRole Id";
            ClubRole clubRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/{invalidClubRoleId}", clubRole);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutClubRoleAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                clubRole.Name = null!;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", clubRole);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutClubRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            ClubRole clubRole = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", clubRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PatchClubRoleAsync

        [TestMethod]
        public async Task PatchClubRoleAsync_WithValidClubRole_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder
                .WithClubId(_clubId)
                .WithActiveStatus(ActiveStatus.Active)
                .Build();
            ClubRole? clubRoleResponse = null;
            string updatedName = "New Club Role Name";
            ActiveStatus updatedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = new()
            {
                new() { Property = nameof(ClubRole.ActiveStatus), Value = updatedStatus },
                new() { Property = nameof(ClubRole.Name), Value = updatedName },
            };
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", updates);
                clubRoleResponse = await result.Content.ReadFromJsonAsync<ClubRole>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubRoleResponse);
                Assert.AreEqual(clubRole.Id, clubRoleResponse.Id);
                Assert.AreEqual(updatedName, clubRoleResponse.Name);
                Assert.AreEqual(updatedStatus, clubRoleResponse.ActiveStatus);
                Assert.AreNotEqual(clubRole.ModifiedOnUtc, clubRoleResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.ClubRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubRoleResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedStatus, getResponse.ActiveStatus);
                Assert.AreEqual(updatedName, getResponse.Name);
                Assert.AreNotEqual(clubRole.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (clubRoleResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubRoleResponse.ToClubRoleEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PatchClubRoleAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            string invalidClubId = "Invalid Club Role Id";
            ClubRole clubRole = _dtoBuilder.Build();
            string updatedName = "New Club Role Name";
            List<PropertyUpdate> updates = new()
            {
                new() { Property = nameof(ClubRole.Name), Value = updatedName }
            };
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{invalidClubId}", updates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchClubRoleAsync_WithInvalidUpdates_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Write";
            ClubRole clubRole = _dtoBuilder.Build();
            string invalidPropertyUpdates = "Invalid Property Updates";
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", invalidPropertyUpdates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchClubRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            ClubRole clubRole = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}", clubRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region DeleteClubRoleAsync

        [TestMethod]
        public async Task DeleteClubRoleAsync_WithValidId_ShouldDeleteRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Delete";
            ClubRole clubRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRole.Id}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

                // Ensure record is actually deleted in DB
                var getResponse = await _dbContext.ClubRoles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubRole.Id);
                Assert.IsNull(getResponse);
            }
            catch (Exception)
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteClubRoleAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Delete";
            string invalidClubRoleId = "Invalid ClubRole Id";
            ClubRole clubRole = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.ClubRoles.Add(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{invalidClubRoleId}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(clubRole.ToClubRoleEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteClubRoleAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "ClubRoles.Delete";
            Guid clubRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubRoleAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid clubRoleId = Guid.NewGuid();

            // Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{AccessControlServiceEndpoints.CurrentVersion}/clubs/{_clubId}/roles/{clubRoleId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion
    }
}
