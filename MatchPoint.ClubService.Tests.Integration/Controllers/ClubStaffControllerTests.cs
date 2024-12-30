using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using System.Net;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.ClubService.Mappers;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Tests.Integration.Controllers
{
    [TestClass]
    public class ClubStaffControllerTests
    {
        private static WebApplicationFactory<Program> _factory = default!;
        private static HttpClient _httpClient = default!;
        private static ClubServiceDbContext _dbContext = null!;
        private ClubStaffEntityBuilder _entityBuilder = default!;
        private ClubStaffBuilder _dtoBuilder = default!;
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
            _entityBuilder = new ClubStaffEntityBuilder();
            _dtoBuilder = new ClubStaffBuilder();
        }

        #region GetClubStaffAsync

        [TestMethod]
        public async Task GetClubStaffAsync_WithNoQueryParameters_ShouldReturnAllRecordsWithDefaultPaging()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity1 = _entityBuilder
                .WithEmail("clubstaff1@test.com")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubStaffEntity clubStaffEntity2 = _entityBuilder
                .WithEmail("clubstaff2@test.com")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubStaffEntity clubStaffEntity3 = _entityBuilder
                .WithEmail("clubstaff3@test.com")
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            // Add a staff for a different club id to ensure it is not included.
            ClubStaffEntity clubStaffEntity4 = _entityBuilder
                .WithEmail("clubstaff4@test.com")
                .WithClubId(Guid.NewGuid())
                .Build();

            try
            {
                // First create test club staff
                _dbContext.ClubStaff.Add(clubStaffEntity1);
                _dbContext.ClubStaff.Add(clubStaffEntity2);
                _dbContext.ClubStaff.Add(clubStaffEntity3);
                _dbContext.ClubStaff.Add(clubStaffEntity4);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<ClubStaff>>();

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
                _dbContext.Remove(clubStaffEntity1);
                _dbContext.Remove(clubStaffEntity2);
                _dbContext.Remove(clubStaffEntity3);
                _dbContext.Remove(clubStaffEntity4);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubStaffAsync_WithValidQueryParameters_ShouldReturnFilteredSortedRecordsWithPaging()
        {
            // Arrange
            int page = 2;
            int pageSize = 1;
            ActiveStatus filterStatus = ActiveStatus.Active;
            ClubStaffEntity clubStaffEntity1 = _entityBuilder
                .WithEmail("clubstaff1@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubStaffEntity clubStaffEntity2 = _entityBuilder
                .WithEmail("clubstaff2@test.com")
                .WithActiveStatus(ActiveStatus.Active)
                .WithClubId(_clubId)
                .Build();
            _entityBuilder = new();
            ClubStaffEntity clubStaffEntity3 = _entityBuilder
                .WithEmail("clubstaff3@test.com")
                .WithActiveStatus(ActiveStatus.Inactive)
                .WithClubId(_clubId)
                .Build();

            try
            {
                // First create test clubs
                _dbContext.ClubStaff.Add(clubStaffEntity2);
                _dbContext.ClubStaff.Add(clubStaffEntity1);
                _dbContext.ClubStaff.Add(clubStaffEntity3);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff" +
                    $"?page={page}&pageSize={pageSize}" +
                    $"&filters[activeStatus]={filterStatus}&orderBy[email]=ascending");
                var pagedResponse = await result.Content.ReadFromJsonAsync<PagedResponse<ClubStaff>>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(pagedResponse);
                Assert.AreEqual(page, pagedResponse.CurrentPage);
                Assert.AreEqual(pageSize, pagedResponse.PageSize);
                Assert.AreEqual(2, pagedResponse.TotalCount);
                Assert.AreEqual(1, pagedResponse.Data.Count());
                Assert.AreEqual(clubStaffEntity2.Email, pagedResponse.Data.First().Email);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubStaffEntity1);
                _dbContext.Remove(clubStaffEntity2);
                _dbContext.Remove(clubStaffEntity3);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetClubStaffAsync_WithInvalidQueryParameters_ShouldReturnBadRequest()
        {
            // Arrange
            string invalidFilters = "Invalid Filters";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff?filters={invalidFilters}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task GetClubStaffAsync_WithInvalidClubId_ShouldReturnNotFound()
        {
            // Arrange
            string clubId = "1";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}/staff");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        #endregion

        #region GetSingleClubStaffAsync

        [TestMethod]
        public async Task GetSingleClubStaffAsync_WithValidId_ShouldReturnSingleRecord()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            ClubStaffEntity clubStaffEntity = _entityBuilder
                .WithId(clubStaffId)
                .WithClubId(_clubId)
                .WithEmail("staff@club.com")
                .Build();

            try
            {
                // First create test club staff
                _dbContext.ClubStaff.Add(clubStaffEntity);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _httpClient.GetAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff/{clubStaffId}");
                var clubStaffResponse = await result.Content.ReadFromJsonAsync<ClubStaff>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.IsNotNull(clubStaffResponse);
                Assert.AreEqual(clubStaffId, clubStaffResponse.Id);
                Assert.AreEqual(clubStaffEntity.Email, clubStaffResponse.Email);
            }
            finally
            {
                // Cleanup
                _dbContext.Remove(clubStaffEntity);
                await _dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetSingleClubStaffAsync_WithInvalidClubId_ShouldReturnNotFound()
        {
            // Arrange
            string clubId = "1";
            Guid clubStaffId = Guid.NewGuid();

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}/staff/{clubStaffId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetSingleClubStaffAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            string clubStaffId = "1";

            // Act
            var result = await _httpClient.GetAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff/{clubStaffId}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        #endregion

        #region PostClubStaffAsync

        [TestMethod]
        public async Task PostClubStaffAsync_WithValidClub_ShouldCreateAndReturnCreatedRecord()
        {
            // Arrange
            ClubStaff clubStaff = _dtoBuilder
                .WithDefaultId()
                .Build();
            ClubStaff? clubStaffResponse = null;

            try
            {
                // Act
                var result = await _httpClient.PostAsJsonAsync(
                    $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff", clubStaff);
                var test = await result.Content.ReadAsStringAsync();
                clubStaffResponse = await result.Content.ReadFromJsonAsync<ClubStaff>();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                Assert.IsNotNull(clubStaffResponse);
                Assert.AreEqual(clubStaff.Email, clubStaffResponse.Email);
                Assert.AreEqual(_clubId, clubStaffResponse.ClubId);

                // Ensure record is actually in DB
                var getResponse = await _dbContext.ClubStaff.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clubStaffResponse.Id);
                Assert.IsNotNull(getResponse);
            }
            finally
            {
                // Cleanup
                if (clubStaffResponse != null)
                {
                    _dbContext.Remove(clubStaffResponse.ToClubStaffEntity());
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task PostClubStaffAsync_WithoutRequiredProperty_ShouldReturnBadRequest()
        {
            // Arrange
            ClubStaff clubStaff = _dtoBuilder.Build();
            clubStaff.Email = null!;

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff", clubStaff);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubStaffAsync_WithInvalidClubStaff_ShouldReturnBadRequest()
        {
            // Arrange
            string clubStaff = "Invalid Club Staff";

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{_clubId}/staff", clubStaff);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task PostClubStaffAsync_WithInvalidClubId_ShouldReturnNotFound()
        {
            // Arrange
            string clubId = "1";
            ClubStaff clubStaff = _dtoBuilder
                .WithDefaultId()
                .Build();

            // Act
            var result = await _httpClient.PostAsJsonAsync(
                $"api/v{ClubServiceEndpoints.CurrentVersion}/clubs/{clubId}/staff", clubStaff);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        #endregion
    }
}
