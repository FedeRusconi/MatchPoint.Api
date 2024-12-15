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
            _dbContext = new(DataContextHelpers.TestingConfiguration);
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
                Assert.AreEqual(clubId,clubResponse.Id);
                Assert.AreEqual(clubEntity.Email,clubResponse.Email);
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

        #endregion

        #region PostClubAsync

        [TestMethod]
        public async Task PostClubAsync_WithValidClub_ShouldCreateAndReturnCreatedRecord()
        {
            #region Arrange
            Club club = _dtoBuilder.WithEmail("club@test.com").Build();
            #endregion

            try
            {
                #region Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs", club);
                string test = await result.Content.ReadAsStringAsync();
                var clubResponse = await result.Content.ReadFromJsonAsync<Club>();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubResponse);
                Assert.AreEqual(club.Id, clubResponse.Id);
                Assert.AreEqual(club.Email, clubResponse.Email);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.Clubs.FindAsync(club.Id);
                Assert.IsNotNull(getResponse);
                #endregion
            }
            finally
            {
                #region Cleanup
                _dbContext.Remove(club.ToClubEntity());
                await _dbContext.SaveChangesAsync();
                #endregion
            }
        }

        #endregion
    }
}
