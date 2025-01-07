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
    public class ClubRepositoryTests
    {
        private ClubRepository _clubRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private ClubServiceDbContext _dbContext = default!;
        private Mock<ILogger<ClubRepository>> _loggerMock = default!;

        private ClubEntityBuilder _clubEntityBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _loggerMock = new();
            _clubRepository = new ClubRepository(_dbContext, _loggerMock.Object);
            _clubEntityBuilder = new();
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        #region CountAsync

        [TestMethod]
        public async Task CountAsync_WithNoFilters_ShouldReturnCountOfAllClubs()
        {
            // Arrange
            var clubEntity1 = _clubEntityBuilder.Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder.Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity3 = _clubEntityBuilder.Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubRepository.CountAsync(_cancellationToken);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup 
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredClubs()
        {
            // Arrange
            var searchName = "Integration Testing Club";
            var searchStatus = ActiveStatus.Active;
            var clubEntity1 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(searchStatus)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(ActiveStatus.Inactive)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity3 = _clubEntityBuilder
                .WithName("Should not be found")
                .WithActiveStatus(searchStatus)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity4 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(searchStatus)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubEntity.Name), searchName},
                { nameof(ClubEntity.ActiveStatus), searchStatus.ToString() }
            };

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity4 = await _clubRepository.CreateAsync(clubEntity4, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubRepository.CountAsync(_cancellationToken, filters);

                // Assert
                Assert.AreEqual(2, resultCount);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity3, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity4, _cancellationToken);
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnClub()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetByIdAsync(clubEntity.Id, _cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Id, result.Id);
                Assert.AreEqual(clubEntity.Name, result.Name);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();

            // Act
            var result = await _clubRepository.GetByIdAsync(clubId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredClubs()
        {
            // Arrange
            var searchName = "Integration Testing Club";
            var searchStatus = ActiveStatus.Active;
            var clubEntity1 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(searchStatus)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(ActiveStatus.Inactive)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity3 = _clubEntityBuilder
                .WithName("Should not be found")
                .WithActiveStatus(searchStatus)
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity4 = _clubEntityBuilder
                .WithName(searchName)
                .WithActiveStatus(searchStatus)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubEntity.Name), searchName},
                { nameof(ClubEntity.ActiveStatus), searchStatus.ToString() }
            };

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity4 = await _clubRepository.CreateAsync(clubEntity4, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, filters: filters, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.Name == searchName && c.ActiveStatus == searchStatus));
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity3, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity4, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedClubs()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(ClubEntity.Name), SortDirection.Ascending } };
            var clubEntity1 = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder
                .WithName("Another. This should come first.")
                .Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().Name == clubEntity2.Name);
                Assert.IsTrue(result.Data.ElementAt(1).Name == clubEntity1.Name);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedClubs()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(ClubEntity.Name), SortDirection.Descending } };
            var clubEntity1 = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder
                .WithName("Another. This should come last.")
                .Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().Name == clubEntity1.Name);
                Assert.IsTrue(result.Data.ElementAt(1).Name == clubEntity2.Name);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedClubs()
        {
            // Arrange
            int pageSize = 1;
            int currentPage = 2;
            var clubEntity1 = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
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
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
            }
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenClubsExist_ShouldReturnClubs()
        {
            // Arrange
            var clubEntity1 = _clubEntityBuilder
                .WithName("Integration Testing Club 1")
                .WithEmail("club1@test.com")
                .Build();
            var clubEntityBuilder2 = new ClubEntityBuilder();
            var clubEntity2 = clubEntityBuilder2
                .WithName("Integration Testing Club 2")
                .WithEmail("club2@test.com")
                .Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRepository.GetAllAsync(_cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(0, result.Count());
                Assert.AreEqual(clubEntity1.Id, result.ElementAt(0).Id);
                Assert.AreEqual(clubEntity2.Id, result.ElementAt(1).Id);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity1, _cancellationToken);
                await _clubRepository.DeleteAsync(clubEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoClubsExist_ShouldReturnEmpty()
        {
            // Act
            var result = await _clubRepository.GetAllAsync(_cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubIsValid_ShouldCreateAndReturn()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            ClubEntity? result = null;

            try
            {
                // Act
                result = await _clubRepository.CreateAsync(clubEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Name, result.Name);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _clubRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubEntity clubEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRepository.CreateAsync(clubEntity, _cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_TransactionIsActive_ShouldNotCommitAutomatically()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            ClubEntity? result = null;

            try
            {
                // Act
                _clubRepository.BeginTransaction();
                var createResult = await _clubRepository.CreateAsync(clubEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(createResult);
                var checkResult = await _clubRepository.GetByIdAsync(createResult.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _clubRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsValid_ShouldUpdateAndReturn()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            string editedName = "This is an edited club";
            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");

                clubEntity.Name = editedName;

                // Act
                var result = await _clubRepository.UpdateAsync(clubEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedName, result.Name);
            }
            finally
            {
                // Cleanup
                await _clubRepository.DeleteAsync(clubEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            // Act
            var result = await _clubRepository.UpdateAsync(clubEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubEntity clubEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRepository.UpdateAsync(clubEntity, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubExists_ShouldDeleteAndReturnEntity()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
            }
            finally
            {
                // Act
                var result = await _clubRepository.DeleteAsync(clubEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Id, result.Id);
                var checkResult = await _clubRepository.GetByIdAsync(clubEntity.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var club = _clubEntityBuilder.Build();

            // Act
            var result = await _clubRepository.DeleteAsync(club, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_TransactionIsActive_ShouldNotCommitAutomatically()
        {
            // Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity, _cancellationToken)
                    ?? throw new Exception("_clubRepository.CreateAsync() returned null");
            }
            finally
            {
                _clubRepository.BeginTransaction();
                // Act
                var result = await _clubRepository.DeleteAsync(clubEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                var checkResult = await _clubRepository.GetByIdAsync(clubEntity.Id, _cancellationToken);
                Assert.IsNotNull(checkResult);

                // Cleanup - Commit to actually delete the test club
                await _clubRepository.CommitTransactionAsync(_cancellationToken);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubEntity clubEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRepository.DeleteAsync(clubEntity, _cancellationToken));
        }

        #endregion
    }
}
