using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
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

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _loggerMock = new();
            _clubRepository = new ClubRepository(_dbContext, _loggerMock.Object);
            _clubEntityBuilder = new();
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
            #region Arrange
            var clubEntity1 = _clubEntityBuilder.Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity2 = _clubEntityBuilder.Build();

            _clubEntityBuilder = new ClubEntityBuilder();
            var clubEntity3 = _clubEntityBuilder.Build();

            try
            {
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3);
                #endregion

                #region Act
                var resultCount = await _clubRepository.CountAsync();
                #endregion

                #region Assert
                Assert.AreEqual(3, resultCount);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                await _clubRepository.DeleteAsync(clubEntity3.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3);
                clubEntity4 = await _clubRepository.CreateAsync(clubEntity4);
                #endregion

                #region Act
                var resultCount = await _clubRepository.CountAsync(filters);
                #endregion

                #region Assert
                Assert.AreEqual(2, resultCount);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                await _clubRepository.DeleteAsync(clubEntity3.Id);
                await _clubRepository.DeleteAsync(clubEntity4.Id);
                #endregion
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnClub()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity);
                #endregion

                #region Act
                var result = await _clubRepository.GetByIdAsync(clubEntity.Id, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Id, result.Id);
                Assert.AreEqual(clubEntity.Name, result.Name);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            #endregion

            #region Act
            var result = await _clubRepository.GetByIdAsync(clubId);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                clubEntity3 = await _clubRepository.CreateAsync(clubEntity3);
                clubEntity4 = await _clubRepository.CreateAsync(clubEntity4);
                #endregion

                #region Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, filters: filters, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.Name == searchName && c.ActiveStatus == searchStatus));
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                await _clubRepository.DeleteAsync(clubEntity3.Id);
                await _clubRepository.DeleteAsync(clubEntity4.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                #endregion

                #region Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, orderBy: orderBy, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().Name == clubEntity2.Name);
                Assert.IsTrue(result.Data.ElementAt(1).Name == clubEntity1.Name);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                #endregion

                #region Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    1, 10, orderBy: orderBy, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().Name == clubEntity1.Name);
                Assert.IsTrue(result.Data.ElementAt(1).Name == clubEntity2.Name);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                #endregion

                #region Act
                var result = await _clubRepository.GetAllWithSpecificationAsync(
                    pageNumber: currentPage, pageSize: pageSize);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(2, result.TotalPages);
                Assert.AreEqual(2, result.CurrentPage);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                #endregion
            }
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenClubsExist_ShouldReturnClubs()
        {
            #region Arrange
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
                clubEntity1 = await _clubRepository.CreateAsync(clubEntity1);
                clubEntity2 = await _clubRepository.CreateAsync(clubEntity2);
                #endregion

                #region Act
                var result = await _clubRepository.GetAllAsync();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(0, result.Count());
                Assert.AreEqual(clubEntity1.Id, result.ElementAt(0).Id);
                Assert.AreEqual(clubEntity2.Id, result.ElementAt(1).Id);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity1.Id);
                await _clubRepository.DeleteAsync(clubEntity2.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoClubsExist_ShouldReturnEmpty()
        {
            #region Act
            var result = await _clubRepository.GetAllAsync();
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            #endregion
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubIsValid_ShouldCreateAndReturn()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            ClubEntity? result = null;
            #endregion

            try
            {
                #region Act
                result = await _clubRepository.CreateAsync(clubEntity);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Name, result.Name);
                Assert.AreNotEqual(default, result.Id);
                #endregion
            }
            finally
            {
                #region Cleanup
                if (result != null)
                {
                    await _clubRepository.DeleteAsync(result.Id);
                }
                #endregion
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubRepository.CreateAsync(clubEntity));
            #endregion
        }

        [TestMethod]
        public async Task CreateAsync_TransactionIsActive_ShouldNotCommitAutomatically()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            ClubEntity? result = null;
            #endregion

            try
            {
                #region Act
                _clubRepository.BeginTransaction();
                var createResult = await _clubRepository.CreateAsync(clubEntity);
                #endregion

                #region Assert
                var checkResult = await _clubRepository.GetByIdAsync(createResult.Id);
                Assert.IsNull(checkResult);
                #endregion
            }
            finally
            {
                #region Cleanup
                if (result != null)
                {
                    await _clubRepository.DeleteAsync(result.Id);
                }
                #endregion
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsValid_ShouldUpdateAndReturn()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            string editedName = "This is an edited club";
            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity);

                clubEntity.Name = editedName;
                #endregion

                #region Act
                var result = await _clubRepository.UpdateAsync(clubEntity);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedName, result.Name);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(clubEntity.Id);
                #endregion
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNotFound_ShouldReturnNull()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            #endregion

            #region Act
            var result = await _clubRepository.UpdateAsync(clubEntity);
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubRepository.UpdateAsync(clubEntity));
            #endregion
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubExists_ShouldDeleteAndReturnEntity()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity);
                #endregion
            }
            finally
            {
                #region Act
                var result = await _clubRepository.DeleteAsync(clubEntity.Id);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubEntity.Id, result.Id);
                var checkResult = await _clubRepository.GetByIdAsync(clubEntity.Id);
                Assert.IsNull(checkResult);
                #endregion
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubDoesNotExist_ShouldReturnNull()
        {
            #region
            var clubId = Guid.NewGuid();
            #endregion

            #region Act
            var result = await _clubRepository.DeleteAsync(clubId);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public async Task DeleteAsync_TransactionIsActive_ShouldNotCommitAutomatically()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            try
            {
                clubEntity = await _clubRepository.CreateAsync(clubEntity);
                #endregion
            }
            finally
            {
                _clubRepository.BeginTransaction();
                #region Act
                var result = await _clubRepository.DeleteAsync(clubEntity.Id);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                var checkResult = await _clubRepository.GetByIdAsync(clubEntity.Id);
                Assert.IsNotNull(checkResult);
                #endregion

                #region Cleanup
                // Commit to actually delete the test club
                await _clubRepository.CommitTransactionAsync();
                #endregion
            }
        }

        #endregion
    }
}
