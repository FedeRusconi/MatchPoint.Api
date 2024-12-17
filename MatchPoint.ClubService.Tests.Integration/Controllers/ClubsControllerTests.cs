using System.Net;
using System.Net.Http.Json;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Mappers;
using MatchPoint.ClubService.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            _entityBuilder = new ClubEntityBuilder();
            _dtoBuilder = new ClubBuilder();
        }

        #region GetClubsAsync

        [TestMethod]
        public async Task GetClubsAsync_WithNoQueryParameters_ShouldReturnAllRecordsWithDefaultPaging()
        {
            #region Arrange
            ClubEntity clubEntity1 = _entityBuilder.WithEmail("club1@test.com").Build();
            ClubEntityBuilder entityBuilder2 = new();
            ClubEntity clubEntity2 = entityBuilder2.WithEmail("club2@test.com").Build();
            ClubEntityBuilder entityBuilder3 = new();
            ClubEntity clubEntity3 = entityBuilder3.WithEmail("club3@test.com").Build();

            try
            {
                // First create test clubs
                _dbContext.Clubs.Add(clubEntity1);
                _dbContext.Clubs.Add(clubEntity2);
                _dbContext.Clubs.Add(clubEntity3);
                await _dbContext.SaveChangesAsync();

                #endregion

                #region Act
                var result = await _httpClient.GetAsync($"api/v{ClubServiceEndpoints.CurrentVersion}/clubs");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<Club>>();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(1, pagedResponse.CurrentPage);
                Assert.AreEqual(Constants.MaxPageSizeAllowed, pagedResponse.PageSize);
                Assert.AreEqual(3, pagedResponse.TotalCount);
                Assert.AreEqual(3, pagedResponse.Data.Count());
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext.Remove(clubEntity1);
                _dbContext.Remove(clubEntity2);
                _dbContext.Remove(clubEntity3);
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task GetClubsAsync_WithValidQueryParameters_ShouldReturnFilteredSortedRecordsWithPaging()
        {
            #region Arrange
            int page = 2;
            int pageSize = 1;
            ActiveStatus filterStatus = ActiveStatus.Active;
            ClubEntity clubEntity1 = _entityBuilder
                .WithEmail("club1@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .Build();
            ClubEntityBuilder entityBuilder2 = new();
            ClubEntity clubEntity2 = entityBuilder2
                .WithEmail("club2@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .Build();
            ClubEntityBuilder entityBuilder3 = new();
            ClubEntity clubEntity3 = entityBuilder3
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

                #endregion

                #region Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs" +
                    $"?page={page}&pageSize={pageSize}" +
                    $"&filters[activeStatus]={filterStatus}&orderBy[email]=ascending");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<Club>>();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(page, pagedResponse.CurrentPage);
                Assert.AreEqual(pageSize, pagedResponse.PageSize);
                Assert.AreEqual(2, pagedResponse.TotalCount);
                Assert.AreEqual(1, pagedResponse.Data.Count());
                Assert.AreEqual(clubEntity2.Email, pagedResponse.Data.First().Email);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext.Remove(clubEntity1);
                _dbContext.Remove(clubEntity2);
                _dbContext.Remove(clubEntity3);
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task GetClubsAsync_WithInvalidQueryParameters_ShouldReturnBadRequest()
        {
            #region Arrange
            string invalidFilters = "Invalid Filters";
            #endregion

            #region Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs?filters={invalidFilters}");
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            #endregion
        }

        #endregion

        #region GetClubAsync

        [TestMethod]
        public async Task GetClubAsync_WithValidId_ShouldReturnSingleRecord()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            ClubEntity clubEntity = _entityBuilder
                .WithId(clubId)
                .WithEmail("club@test.com")
                .Build();

            try
            {
                // First create test club
                _dbContext.Clubs.Add(clubEntity);
                await _dbContext.SaveChangesAsync();

                #endregion

                #region Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");
                var clubResponse = await result.Content.ReadFromJsonAsync<Club>();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubResponse);
                Assert.AreEqual(clubId, clubResponse.Id);
                Assert.AreEqual(clubEntity.Email, clubResponse.Email);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext.Remove(clubEntity);
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task GetClubAsync_WithInvalidId_ShouldReturnNotFound()
        {
            #region Arrange
            string clubId = "1";
            #endregion

            #region Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}");
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            #endregion
        }

        #endregion

        #region PostClubAsync

        [TestMethod]
        public async Task PostClubAsync_WithValidClub_ShouldCreateAndReturnCreatedRecord()
        {
            #region Arrange
            Club club = _dtoBuilder
                .WithDefaultId()
                .Build();
            Club? clubResponse = null;
            #endregion

            try
            {
                #region Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                Assert.IsNotNull(clubResponse);                
                Assert.AreEqual(club.Email, clubResponse.Email);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubResponse.Id);
                Assert.IsNotNull(getResponse);
                #endregion
            }
            finally
            {
                #region Cleanup
                if (clubResponse != null)
                {
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }                
                #endregion
            }
        }

        [TestMethod]
        public async Task PostClubAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            club.Email = null!;
            #endregion

            #region Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            #endregion
        }

        [TestMethod]
        public async Task PostClubAsync_WithInvalidClub_ShouldReturnBadRequest()
        {
            #region Arrange
            string club = "Invalid Club";
            #endregion

            #region Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            #endregion
        }

        #endregion

        #region PutClubAsync

        [TestMethod]
        public async Task PutClubAsync_WithValidClub_ShouldUpdateAndReturnRecord()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            Club? clubResponse = null;
            string updatedEmail = "changed@email.com";
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                club.Email = updatedEmail;
                #endregion

                #region Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();
                #endregion

                #region Assert
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
                #endregion
            }
            finally
            {
                #region Cleanup
                if (clubResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }
                #endregion
            }
        }

        [TestMethod]
        public async Task PutClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            #region Arrange
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion

                #region Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}", club);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task PutClubAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                club.Email = null!;
                #endregion

                #region Act
                var result = await _httpClient.PutAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", club);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        #endregion

        #region PatchClubAsync

        [TestMethod]
        public async Task PatchClubAsync_WithValidClub_ShouldUpdateAndReturnRecord()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            Club? clubResponse = null;
            string updatedName = "New Club Name";
            string updatedEmail = "changed@email.com";
            List<PropertyUpdate> updates = new()
            {
                new() { Property = nameof(Club.Email), Value = updatedEmail },
                new() { Property = nameof(Club.Name), Value = updatedName },
            };
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();

                club.Email = updatedEmail;
                #endregion

                #region Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", updates);
                clubResponse = await result.Content.ReadFromJsonAsync<Club>();
                #endregion

                #region Assert
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
                #endregion
            }
            finally
            {
                #region Cleanup
                if (clubResponse != null)
                {
                    _dbContext = new(DataContextHelpers.TestingConfiguration);
                    _dbContext.Remove(clubResponse.ToClubEntity());
                    await _dbContext.SaveChangesAsync();
                }
                #endregion
            }
        }

        [TestMethod]
        public async Task PatchClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            #region Arrange
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            string updatedName = "New Club Name";
            string updatedEmail = "changed@email.com";
            List<PropertyUpdate> updates = new()
            {
                new() { Property = nameof(Club.Email), Value = updatedEmail },
                new() { Property = nameof(Club.Name), Value = updatedName },
            };
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion

                #region Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}", updates);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task PatchClubAsync_WithInvalidUpdates_ShouldReturnBadRequest()
        {
            #region Arrange            
            Club club = _dtoBuilder.Build();
            string invalidPropertyUpdates = "Invalid Property Updates";
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion

                #region Act
                var result = await _httpClient.PatchAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}", invalidPropertyUpdates);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        #endregion

        #region DeleteClubAsync

        [TestMethod]
        public async Task DeleteClubAsync_WithValidId_ShouldDeleteRecord()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion

                #region Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}");
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

                // Ensure record is actually deleted in DB
                var getResponse = await _dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == club.Id);
                Assert.IsNull(getResponse);
                #endregion
            }
            catch (Exception)
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithInvalidId_ShouldReturnBadRequest()
        {
            #region Arrange
            string invalidClubId = "Invalid Club Id";
            Club club = _dtoBuilder.Build();
            try
            {
                // Create the record to test the update
                _dbContext.Clubs.Add(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion

                #region Act
                var result = await _httpClient.DeleteAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{invalidClubId}");
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                #endregion
            }
            catch (Exception)
            {
                #region Cleanup
                _dbContext = new(DataContextHelpers.TestingConfiguration);
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        [TestMethod]
        public async Task DeleteClubAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            #region Arrange
            Club club = _dtoBuilder.Build();
            #endregion

            #region Act
            var result = await _httpClient.DeleteAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{club.Id}");
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            #endregion
        }

        #endregion
    }
}
