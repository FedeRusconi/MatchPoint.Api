using System.Net;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
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
using Microsoft.Graph;
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
        private readonly Guid _clubId = Guid.NewGuid();
        private CancellationToken _cancellationToken = default!;

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
            _cancellationToken = new CancellationToken();

            _azureAdServiceMock.SetupGet(s => s.CurrentUserId).Returns(Guid.NewGuid());
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetByIdAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubStaffId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((ClubStaffEntity?)null);

            // Act
            var result = await _clubStaffService.GetByIdAsync(_clubId, clubStaffId, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            // Assign a different club id for the test
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetByIdAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldReturnSuccessResult()
        {
            // Arrange
            int pageNumber = 3;
            int pageSize = 10;
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            PagedResponse<ClubStaffEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [clubStaffEntity]
            };

            _clubStaffRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, _cancellationToken, It.IsAny<Dictionary<string, string>>(), null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, pageNumber, pageSize, _cancellationToken);

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
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
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
                    _cancellationToken,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, filters: filters);

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
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
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
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, orderBy: orderBy);

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
        [DataRow(1, Api.Shared.Common.Utilities.Constants.MaxPageSizeAllowed + 1)]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsInvalid_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            // Act
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, pageNumber, pageSize, _cancellationToken);

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
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, filters: filters);

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
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, orderBy: orderBy);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            azureAdUser.Id = Guid.NewGuid().ToString();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _azureAdServiceMock
                .Setup(s => s.CreateUserAsync(It.IsAny<User>(), _cancellationToken, null))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubStaffEntity>(), _cancellationToken))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(_clubId, clubStaffEntity, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubStaffEntity.Email, result.Data.Email);
            Assert.AreEqual(_clubId, result.Data.ClubId);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
        }

        [TestMethod]
        public async Task CreateAsync_WhenAzureAdOperationFails_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var exception = new HttpRequestException("This is a test error", null, HttpStatusCode.Conflict);
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.CreateUserAsync(
                    It.IsAny<User>(), _cancellationToken, It.IsAny<GraphServiceClient>()))
                .Throws(exception)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(_clubId, clubStaffEntity, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffEmailIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(_clubId, clubStaffEntity, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            azureAdUser.Id = Guid.NewGuid().ToString();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };

            _clubStaffRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _azureAdServiceMock
                .Setup(s => s.CreateUserAsync(It.IsAny<User>(), _cancellationToken, null))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubStaffEntity>(), _cancellationToken))
                .ReturnsAsync((ClubStaffEntity?)null)
                .Verifiable(Times.Once);
            // This is the rollback operation
            _azureAdServiceMock
                .Setup(s => s.DeleteUserAsync(It.IsAny<Guid>(), _cancellationToken, null))
                .ReturnsAsync(true)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.CreateAsync(_clubId, clubStaffEntity, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffService.CreateAsync(_clubId, clubStaffEntity, _cancellationToken));
        }

        #endregion

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingPropertiesAndReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            string editedFirstName = "First Edited";
            string editedPhone = "Updated Phone";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubStaffEntity.FirstName), editedFirstName),
                new PropertyUpdate(nameof(clubStaffEntity.PhoneNumber), editedPhone)
            ];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(s => s.GetUserByIdAsync(
                    clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(s => s.UpdateUserAsync(
                    It.Is<User>(u => u.Id == azureAdUser.Id && u.GivenName == editedFirstName && u.MobilePhone == editedPhone),
                    _cancellationToken,
                    It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubStaffEntity>(), _cancellationToken))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _clubStaffService.PatchAsync(_clubId, clubStaffEntity.Id, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(editedFirstName, result.Data.FirstName);
            Assert.AreEqual(editedPhone, result.Data.PhoneNumber);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubStaffIsNotFoundInDb_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(ClubStaffEntity.FirstName), "Test")];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffId, _cancellationToken, false))
                .ReturnsAsync((ClubStaffEntity?)null)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(s => s.GetUserByIdAsync(
                clubStaffId, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(new User())
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.PatchAsync(_clubId, clubStaffId, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(s => s.GetUserByIdAsync(
                clubStaffId, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubStaffIsNotFoundInAzure_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            var clubStaffEntity = _clubStaffEntityBuilder.WithId(clubStaffId).Build();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(ClubStaffEntity.FirstName), "Test")];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.GetUserByIdAsync(
                clubStaffId, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync((User?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.PatchAsync(
                _clubId, clubStaffId, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(s => s.GetUserByIdAsync(
                clubStaffId, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenPropertyUpdatesAreInvalid_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            List<PropertyUpdate> updates = [
                new PropertyUpdate("Invalid Property", "Value")
            ];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.GetUserByIdAsync(
                    clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.PatchAsync(_clubId, clubStaffEntity.Id, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(s => s.GetUserByIdAsync(
                clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            string editedFirstName = "First Edited";
            string editedPhone = "Updated Phone";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubStaffEntity.FirstName), editedFirstName),
                new PropertyUpdate(nameof(clubStaffEntity.PhoneNumber), editedPhone)
            ];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.GetUserByIdAsync(
                clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.UpdateUserAsync(
                    It.IsAny<User>(),
                    _cancellationToken,
                    It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                // Called twice: first is the updated, second is the rollback
                .Verifiable(Times.Exactly(2));
            _clubStaffRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubStaffEntity>(), _cancellationToken))
                .ReturnsAsync((ClubStaffEntity?)null)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _clubStaffService.PatchAsync(_clubId, clubStaffEntity.Id, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenAzureAdOperationFails_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            var exception = new HttpRequestException("This is a test error", null, HttpStatusCode.Conflict);
            string editedFirstName = "First Edited";
            string editedPhone = "Updated Phone";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubStaffEntity.FirstName), editedFirstName),
                new PropertyUpdate(nameof(clubStaffEntity.PhoneNumber), editedPhone)
            ];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.GetUserByIdAsync(
                    clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.UpdateUserAsync(
                    It.IsAny<User>(),
                    _cancellationToken,
                    It.IsAny<GraphServiceClient>()))
                .Throws(exception)
                .Verifiable(Times.Once());

            // Act
            var result = await _clubStaffService.PatchAsync(
                _clubId, clubStaffEntity.Id, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(Guid.NewGuid()).Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            string editedFirstName = "First Edited";
            string editedPhone = "Updated Phone";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubStaffEntity.FirstName), editedFirstName),
                new PropertyUpdate(nameof(clubStaffEntity.PhoneNumber), editedPhone)
            ];

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.GetUserByIdAsync(
                clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(azureAdUser)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.PatchAsync(
                _clubId, clubStaffEntity.Id, updates, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(s => s.GetUserByIdAsync(
                clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffService.PatchAsync(
                    _clubId, clubStaffId, updates, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var azureAdUser = clubStaffEntity.ToAzureAdUser();

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.DeleteAsync(clubStaffEntity, _cancellationToken))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(repo => repo.DeleteUserAsync(
                    clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .ReturnsAsync(true);

            // Act
            var result = await _clubStaffService.DeleteAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(repo => repo.DeleteUserAsync(
                clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubStaffEntity.Id, result.Data.Id);
            Assert.AreEqual(clubStaffEntity.FirstName, result.Data.FirstName);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync((ClubStaffEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.DeleteAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenRepositoryOperationFails_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var exception = new HttpRequestException("This is a test error", null, HttpStatusCode.Conflict);

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.DeleteAsync(clubStaffEntity, _cancellationToken))
                .ReturnsAsync((ClubStaffEntity)null!)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.DeleteAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenAzureAdOperationFails_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(_clubId).Build();
            var exception = new HttpRequestException("This is a test error", null, HttpStatusCode.Conflict);

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _clubStaffRepositoryMock.Setup(repo => repo.DeleteAsync(clubStaffEntity, _cancellationToken))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);
            _azureAdServiceMock.Setup(
                    repo => repo.DeleteUserAsync(clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()))
                .Throws(exception);
            // This is the rollback operation
            _clubStaffRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubStaffEntity>(), _cancellationToken))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.DeleteAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            _azureAdServiceMock.Verify(
                repo => repo.DeleteUserAsync(clubStaffEntity.Id, _cancellationToken, It.IsAny<GraphServiceClient>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _clubStaffRepositoryMock.Setup(repo => repo.GetByIdAsync(clubStaffEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubStaffEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubStaffService.DeleteAsync(_clubId, clubStaffEntity.Id, _cancellationToken);

            // Assert
            _clubStaffRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion
    }
}
