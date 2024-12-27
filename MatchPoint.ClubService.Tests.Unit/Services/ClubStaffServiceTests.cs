using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data.Factories;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using MatchPoint.ClubService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Services
{
    [TestClass]
    public class ClubStaffServiceTests
    {
        private Mock<IClubStaffRepository> _clubStaffRepositoryMock = default!;
        private Mock<IAzureAdService> _azureAdServiceMock = default!;
        private Mock<ILogger<ClubStaffService>> _loggerMock = default!;
        private IAzureAdUserFactory _azureAdUserFactory = default!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private ClubStaffEntityBuilder _clubStaffEntityBuilder = default!;
        private ClubStaffService _clubStaffService = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _clubStaffRepositoryMock = new();
            _azureAdServiceMock = new();
            _loggerMock = new();
            _azureAdUserFactory = new AzureAdUserFactory();
            _clubStaffEntityBuilder = new();
            _clubStaffService = new(
                _clubStaffRepositoryMock.Object,
                _azureAdServiceMock.Object,
                _azureAdUserFactory,
                _configuration,
                _loggerMock.Object);
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, It.IsAny<bool>()))
                .ReturnsAsync(clubStaffEntity);

            // Act
            var result = await _clubStaffService.GetByIdAsync(clubStaffEntity.Id);

            // Assert
            _clubStaffRepositoryMock.Verify(repo => repo.GetByIdAsync(clubStaffEntity.Id, false), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubId, It.IsAny<bool>()))
                .ReturnsAsync((ClubStaffEntity?)null);

            // Act
            var result = await _clubStaffService.GetByIdAsync(clubId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldReturnSuccessResult()
        {
            // Arrange
            int pageNumber = 3;
            int pageSize = 10;
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            PagedResponse<ClubStaffEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [clubStaffEntity]
            };

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(pageNumber, pageSize, null, null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(pageNumber, pageSize);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFiltersAndReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            Dictionary<string, string> filters = new()
            {
                { nameof(ClubStaffEntity.FirstName), "Test" },
                { nameof(ClubStaffEntity.Email), "test@test.com" },
                { nameof(ClubStaffEntity.ActiveStatus), ActiveStatus.Active.ToString() }
            };
            PagedResponse<ClubStaffEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubStaffEntity]
            };

            _clubStaffRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    expectedResponse.CurrentPage,
                    expectedResponse.PageSize,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(1, 500, filters: filters);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSortingAndReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(ClubStaffEntity.FirstName), SortDirection.Descending }
            };
            PagedResponse<ClubStaffEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubStaffEntity]
            };

            _clubStaffRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    null,
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(1, 500, orderBy: orderBy);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Constants.MaxPageSizeAllowed + 1)]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsInvalid_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(pageNumber, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreInvalid_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, string> filters = new()
            {
                { "NonExistentProperty", "Test" }
            };

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(1, 500, filters: filters);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsInvalid_ShouldReturnFailResult()
        {
            // Arrange=
            Dictionary<string, SortDirection> orderBy = new()
            {
                { "NonExistentProperty", SortDirection.Descending }
            };

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(1, 500, orderBy: orderBy);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            azureAdUser.Id = Guid.NewGuid().ToString();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _azureAdServiceMock
                .Setup(s => s.CreateUserAsync(It.IsAny<User>(), null))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubStaffEntity>()))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(clubStaffEntity);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubStaffEntity.Email, result.Data.Email);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIdIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CreateAsync(clubStaffEntity))
                .ReturnsAsync((ClubStaffEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(clubStaffEntity);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubEmailIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(clubStaffEntity);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffService.CreateAsync(clubStaffEntity));
        }

        #endregion
    }
}
