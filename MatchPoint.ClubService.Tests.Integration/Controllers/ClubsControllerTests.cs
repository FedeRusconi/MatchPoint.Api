using System.Net;
using System.Net.Http.Json;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Extensions;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Mappers;
using MatchPoint.ClubService.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Tests.Integration.Controllers
{
    [TestClass]
    public class ClubsControllerTests
    {
        private static WebApplicationFactory<Program> _factory = default!;
        private static HttpClient _httpClient = default!;
        private static ClubServiceDbContext _dbContext = null!;
        private ClubEntityBuilder _entityBuilder = default!;
        private ClubBuilder _dtoBuilder = default!;
        private static readonly Guid _clubId = Guid.NewGuid();
        private static readonly Guid _userRoleId = Guid.NewGuid();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestAuthHandler.ClubRoleId = _userRoleId;
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
            _entityBuilder = new ClubEntityBuilder();
            _dtoBuilder = new ClubBuilder();

            // Calls custom Extension method to set up a test http client for tests
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpClient.Dispose();
        }

        #region GetClubsAsync

        [TestMethod]
        public async Task GetClubsAsync_WithNoQueryParameters_ShouldReturnAllRecordsWithDefaultPaging()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            ClubEntity clubEntity1 = _entityBuilder.WithEmail("club1@test.com").Build();
            _entityBuilder = new();
            ClubEntity clubEntity2 = _entityBuilder.WithEmail("club2@test.com").Build();
            _entityBuilder = new();
            ClubEntity clubEntity3 = _entityBuilder.WithEmail("club3@test.com").Build();

            try
            {
                // First create test clubs
                _dbContext.Clubs.Add(clubEntity1);
                _dbContext.Clubs.Add(clubEntity2);
                _dbContext.Clubs.Add(clubEntity3);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync($"api/v{ClubServiceEndpoints.CurrentVersion}/clubs");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<Club>>();

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
                _dbContext.Remove(clubEntity1);
                _dbContext.Remove(clubEntity2);
                _dbContext.Remove(clubEntity3);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubsAsync_WithValidQueryParameters_ShouldReturnFilteredSortedRecordsWithPaging()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            int page = 2;
            int pageSize = 1;
            ActiveStatus filterStatus = ActiveStatus.Active;
            ClubEntity clubEntity1 = _entityBuilder
                .WithEmail("club1@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .Build();
            _entityBuilder = new();
            ClubEntity clubEntity2 = _entityBuilder
                .WithEmail("club2@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .Build();
            _entityBuilder = new();
            ClubEntity clubEntity3 = _entityBuilder
                .WithEmail("club3@test.com")
                .WithActiveStatus(ActiveStatus.Inactive)
                .Build();

            try
            {
                // First create test clubs
                _dbContext.Clubs.Add(clubEntity2);
                _dbContext.Clubs.Add(clubEntity1);
                _dbContext.Clubs.Add(clubEntity3);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs" +
                    $"?page={page}&pageSize={pageSize}" +
                    $"&filters[activeStatus]={filterStatus}&orderBy[email]=ascending");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<Club>>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(page, pagedResponse.CurrentPage);
                Assert.AreEqual(pageSize, pagedResponse.PageSize);
                Assert.AreEqual(2, pagedResponse.TotalCount);
                Assert.AreEqual(1, pagedResponse.Data.Count());
                Assert.AreEqual(clubEntity2.Email, pagedResponse.Data.First().Email);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubEntity1);
                _dbContext.Remove(clubEntity2);
                _dbContext.Remove(clubEntity3);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubsAsync_WithInvalidQueryParameters_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            string invalidFilters = "Invalid Filters";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs?filters={invalidFilters}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubsAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubsAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";

            // Act
            var result = await _httpClient.GetAsync($"api/v{ClubServiceEndpoints.CurrentVersion}/clubs");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region GetClubAsync

        [TestMethod]
        public async Task GetClubAsync_WithValidId_ShouldReturnSingleRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            ClubEntity clubEntity = _entityBuilder
                .WithId(_clubId)
                .WithEmail("club@test.com")
                .Build();

            try
            {
                // First create test club
                _dbContext.Clubs.Add(clubEntity);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}");
                var clubResponse = await result.Content.ReadFromJsonAsync<Club>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubResponse);
                Assert.AreEqual(_clubId, clubResponse.Id);
                Assert.AreEqual(clubEntity.Email, clubResponse.Email);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubEntity);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            string clubId = "Invalid Id";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            Guid clubId = Guid.NewGuid();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubAsync_WithInsufficientRoleCapabilities_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Read";
            Guid clubId = Guid.NewGuid();
            // Redefine a HttpClient with insufficient permissions
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                // Insufficient
                RoleCapabilityAction.None);

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid clubId = Guid.NewGuid();

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PostClubAsync

        [TestMethod]
        public async Task PostClubAsync_WithValidClub_ShouldCreateAndReturnCreatedRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder
                .WithDefaultId()
                .Build();
            Club? clubResponse = null;

            try
            {
                // Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                Assert.IsNotNull(clubResponse);                
                Assert.AreEqual(club.Email, clubResponse.Email);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubResponse.Id);
                Assert.IsNotNull(getResponse);
            }
            finally
            {
                // Cleanup
                if (clubResponse != null)
                {
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PostClubAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.Build();
            club.Email = null!;

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubAsync_WithInvalidClub_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            string club = "Invalid Club";

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder
                .WithDefaultId()
                .Build();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Club club = _dtoBuilder
                .WithDefaultId()
                .Build();

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PutClubAsync

        [TestMethod]
        public async Task PutClubAsync_WithValidClub_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.WithId(_clubId).Build();
            Club? clubResponse = null;
            string updatedEmail = "changed@email.com";
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                club.Email = updatedEmail;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubResponse);
                Assert.AreEqual(club.Id, clubResponse.Id);
                Assert.AreEqual(updatedEmail, clubResponse.Email);
                Assert.AreNotEqual(club.ModifiedOnUtc, clubResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedEmail, getResponse.Email);
                Assert.AreNotEqual(club.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (clubResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PutClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}", club);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutClubAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.WithId(_clubId).Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                club.Email = null!;

                // Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PutClubAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.Build();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PutClubAsync_WithInsufficientRoleCapabilities_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.Build();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                // Insufficient
                RoleCapabilityAction.None);

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task PutClubAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Club club = _dtoBuilder.Build();

            // Act
            var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region PatchClubAsync

        [TestMethod]
        public async Task PatchClubAsync_WithValidClub_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.WithId(_clubId).Build();
            Club? clubResponse = null;
            string updatedName = "New Club Name";
            string updatedEmail = "changed@email.com";
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(Club.Email), Value = updatedEmail },
                new() { Property = nameof(Club.Name), Value = updatedName },
            ];
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", updates);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubResponse);
                Assert.AreEqual(club.Id, clubResponse.Id);
                Assert.AreEqual(updatedName, clubResponse.Name);
                Assert.AreEqual(updatedEmail, clubResponse.Email);
                Assert.AreNotEqual(club.ModifiedOnUtc, clubResponse.ModifiedOnUtc);

                // Ensure record is actually updated in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubResponse.Id);
                Assert.IsNotNull(getResponse);
                Assert.AreEqual(updatedEmail, getResponse.Email);
                Assert.AreEqual(updatedName, getResponse.Name);
                Assert.AreNotEqual(club.ModifiedOnUtc, getResponse.ModifiedOnUtc);
            }
            finally
            {
                // Cleanup
                if (clubResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PatchClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            string updatedName = "New Club Name";
            string updatedEmail = "changed@email.com";
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(Club.Email), Value = updatedEmail },
                new() { Property = nameof(Club.Name), Value = updatedName },
            ];
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}", updates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchClubAsync_WithInvalidUpdates_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Club club = _dtoBuilder.WithId(_clubId).Build();
            string invalidPropertyUpdates = "Invalid Property Updates";
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", invalidPropertyUpdates);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task PatchClubAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(Club.Email), Value = "updated@email.com" }
            ];
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}", updates);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task PatchClubAsync_WithInsufficientRoleCapabilities_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Write";
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(Club.Email), Value = "updated@email.com" }
            ];
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                // Insufficient
                RoleCapabilityAction.None);

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}", updates);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task PatchClubAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates =
            [
                new() { Property = nameof(Club.Email), Value = "updated@email.com" }
            ];

            // Act
            var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}", updates);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion

        #region DeleteClubAsync

        [TestMethod]
        public async Task DeleteClubAsync_WithValidId_ShouldDeleteRecord()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Delete";
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

                // Ensure record is actually deleted in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == club.Id);
                Assert.IsNull(getResponse);
            }
            catch (Exception)
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Delete";
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            finally
            {
                // Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Delete";

            // Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubAsync_UnauthenticatedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Delete";
            Guid clubId = Guid.NewGuid();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                RoleCapabilityAction.ReadWriteDelete,
                authenticated: false);

            // Act
            var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithInsufficientRoleCapabilities_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Clubs.Delete";
            Guid clubId = Guid.NewGuid();
            // Redefine a HttpClient without an authenticated user
            _httpClient = _factory.GetTestHttpClient(
                _clubId,
                _userRoleId,
                RoleCapabilityFeature.ManageClub,
                // Insufficient
                RoleCapabilityAction.None);

            // Act
            var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithWrongScopes_ShouldReturnForbidden()
        {
            // Arrange
            TestAuthHandler.Scopes = "Wrong.Scopes";
            Guid clubId = Guid.NewGuid();

            // Act
            var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        #endregion
    }
}
