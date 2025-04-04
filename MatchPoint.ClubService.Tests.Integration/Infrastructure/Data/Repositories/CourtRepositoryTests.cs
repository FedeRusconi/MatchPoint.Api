using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.ClubService.Tests.Integration.Infrastructure.Data.Repositories
{
    [TestClass]
    public class CourtRepositoryTests
    {
        private CourtRepository _courtRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private ClubServiceDbContext _dbContext = default!;
        private Mock<ILogger<CourtRepository>> _loggerMock = default!;

        private CourtEntityBuilder _courtEntityBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _loggerMock = new();
            _courtRepository = new CourtRepository(_dbContext, _loggerMock.Object);
            _courtEntityBuilder = new();
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        #region CountAsync

        [TestMethod]
        public async Task CountAsync_WithNoFilters_ShouldReturnCountOfAllCourts()
        {
            // Arrange
            var courtEntity1 = _courtEntityBuilder.Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder.Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity3 = _courtEntityBuilder.Build();

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity3 = await _courtRepository.CreateAsync(courtEntity3, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _courtRepository.CountAsync(_cancellationToken);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup 
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredCourts()
        {
            // Arrange
            var searchClubId = Guid.NewGuid();
            var courtEntity1 = _courtEntityBuilder
                .WithClubId(searchClubId)
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder
                .WithClubId(searchClubId)
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity3 = _courtEntityBuilder
                .WithClubId(Guid.NewGuid())
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity4 = _courtEntityBuilder
                .WithClubId(Guid.NewGuid())
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(CourtEntity.ClubId), searchClubId.ToString()}
            };

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity3 = await _courtRepository.CreateAsync(courtEntity3, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity4 = await _courtRepository.CreateAsync(courtEntity4, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _courtRepository.CountAsync(_cancellationToken, filters);

                // Assert
                Assert.AreEqual(2, resultCount);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity3, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity4, _cancellationToken);
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnCourt()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();

            try
            {
                courtEntity = await _courtRepository.CreateAsync(courtEntity, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetByIdAsync(
                    courtEntity.Id, _cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(courtEntity.Id, result.Id);
                Assert.AreEqual(courtEntity.Name, result.Name);
                Assert.AreEqual(courtEntity.ClubId, result.ClubId);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid courtId = Guid.NewGuid();

            // Act
            var result = await _courtRepository.GetByIdAsync(courtId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredCourts()
        {
            // Arrange
            var searchClubId = Guid.NewGuid();
            var courtEntity1 = _courtEntityBuilder
                .WithClubId(searchClubId)
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder
                .WithClubId(searchClubId)
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity3 = _courtEntityBuilder
                .WithClubId(Guid.NewGuid())
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity4 = _courtEntityBuilder
                .WithClubId(Guid.NewGuid())
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(CourtEntity.ClubId), searchClubId.ToString()}
            };

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity3 = await _courtRepository.CreateAsync(courtEntity3, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity4 = await _courtRepository.CreateAsync(courtEntity4, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, filters: filters, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.ClubId == searchClubId));
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity3, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity4, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedCourts()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(CourtEntity.Name), SortDirection.Ascending }
            };
            var courtEntity1 = _courtEntityBuilder
                .WithName("Court 2")
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder
                .WithName("Court 1")
                .Build();

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(courtEntity2.Name, result.Data.First().Name);
                Assert.AreEqual(courtEntity1.Name, result.Data.ElementAt(1).Name);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedCourts()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(CourtEntity.Name), SortDirection.Descending }
            };
            var courtEntity1 = _courtEntityBuilder
                .WithName("Court 2")
                .Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder
                .WithName("Court 1")
                .Build();

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(courtEntity1.Name, result.Data.First().Name);
                Assert.AreEqual(courtEntity2.Name, result.Data.ElementAt(1).Name);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedCourts()
        {
            // Arrange
            int pageSize = 1;
            int currentPage = 2;
            var courtEntity1 = _courtEntityBuilder.Build();

            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder.Build();

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetAllWithSpecificationAsync(
                    pageNumber: currentPage, pageSize: pageSize, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(2, result.TotalPages);
                Assert.AreEqual(2, result.CurrentPage);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
            }
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenCourtsExist_ShouldReturnCourts()
        {
            // Arrange
            var courtEntity1 = _courtEntityBuilder.Build();
            _courtEntityBuilder = new CourtEntityBuilder();
            var courtEntity2 = _courtEntityBuilder.Build();

            try
            {
                courtEntity1 = await _courtRepository.CreateAsync(courtEntity1, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
                courtEntity2 = await _courtRepository.CreateAsync(courtEntity2, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                // Act
                var result = await _courtRepository.GetAllAsync(_cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(0, result.Count());
                Assert.AreEqual(courtEntity1.Id, result.ElementAt(0).Id);
                Assert.AreEqual(courtEntity2.Id, result.ElementAt(1).Id);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity1, _cancellationToken);
                await _courtRepository.DeleteAsync(courtEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoCourtsExist_ShouldReturnEmpty()
        {
            // Act
            var result = await _courtRepository.GetAllAsync(_cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenCourtIsValid_ShouldCreateAndReturn()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();
            CourtEntity? result = null;

            try
            {
                // Act
                result = await _courtRepository.CreateAsync(courtEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(courtEntity.ClubId, result.ClubId);
                Assert.AreEqual(courtEntity.Name, result.Name);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _courtRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenCourtIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CourtEntity courtEntity = null!;

            // Act
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(
                () => _courtRepository.CreateAsync(courtEntity, _cancellationToken));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenCourtIsValid_ShouldUpdateAndReturn()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();
            string editedName = "This is an edited court name";
            try
            {
                courtEntity = await _courtRepository.CreateAsync(courtEntity, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");

                courtEntity.Name = editedName;

                // Act
                var result = await _courtRepository.UpdateAsync(courtEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedName, result.Name);
            }
            finally
            {
                // Cleanup
                await _courtRepository.DeleteAsync(courtEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCourtIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();

            // Act
            var result = await _courtRepository.UpdateAsync(courtEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCourtIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CourtEntity courtEntity = null!;

            // Act
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(
                () => _courtRepository.UpdateAsync(courtEntity, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenCourtExists_ShouldDeleteAndReturnEntity()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();

            try
            {
                courtEntity = await _courtRepository.CreateAsync(courtEntity, _cancellationToken)
                    ?? throw new Exception("_courtRepository.CreateAsync() returned null");
            }
            finally
            {
                // Act
                var result = await _courtRepository.DeleteAsync(courtEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(courtEntity.Id, result.Id);
                var checkResult = await _courtRepository.GetByIdAsync(courtEntity.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCourtDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var courtEntty = _courtEntityBuilder.Build();

            // Act
            var result = await _courtRepository.DeleteAsync(courtEntty, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCourtIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CourtEntity courtEntity = null!;

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(
                () => _courtRepository.DeleteAsync(courtEntity, _cancellationToken));
        }

        #endregion
    }
}
