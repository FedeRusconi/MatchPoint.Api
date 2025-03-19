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
    public class ClubStaffRepositoryTests
    {
        private ClubStaffRepository _clubStaffRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private ClubServiceDbContext _dbContext = default!;
        private Mock<ILogger<ClubStaffRepository>> _loggerMock = default!;

        private ClubStaffEntityBuilder _clubStaffEntityBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _loggerMock = new();
            _clubStaffRepository = new ClubStaffRepository(_dbContext, _loggerMock.Object);
            _clubStaffEntityBuilder = new();
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        #region CountAsync

        [TestMethod]
        public async Task CountAsync_WithNoFilters_ShouldReturnCountOfAllClubStaff()
        {
            // Arrange
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity3 = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubStaffRepository.CountAsync(_cancellationToken);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup 
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredClubStaff()
        {
            // Arrange
            var searchClubId = Guid.NewGuid();
            var searchRoleId = Guid.NewGuid();
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(searchRoleId)
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(Guid.NewGuid())
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity3 = _clubStaffEntityBuilder
                .WithClubId(Guid.NewGuid())
                .WithRoleId(searchRoleId)
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity4 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(searchRoleId)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubStaffEntity.ClubId), searchClubId.ToString()},
                { nameof(ClubStaffEntity.RoleId), searchRoleId.ToString() }
            };

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity4 = await _clubStaffRepository.CreateAsync(clubStaffEntity4, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubStaffRepository.CountAsync(_cancellationToken, filters);

                // Assert
                Assert.AreEqual(2, resultCount);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity4, _cancellationToken);
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnClubStaff()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetByIdAsync(
                    clubStaffEntity.Id, _cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.Id, result.Id);
                Assert.AreEqual(clubStaffEntity.ClubId, result.ClubId);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();

            // Act
            var result = await _clubStaffRepository.GetByIdAsync(clubStaffId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredClubStaff()
        {
            // Arrange
            var searchClubId = Guid.NewGuid();
            var searchRoleId = Guid.NewGuid();
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(searchRoleId)
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(Guid.NewGuid())
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity3 = _clubStaffEntityBuilder
                .WithClubId(Guid.NewGuid())
                .WithRoleId(searchRoleId)
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity4 = _clubStaffEntityBuilder
                .WithClubId(searchClubId)
                .WithRoleId(searchRoleId)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubStaffEntity.ClubId), searchClubId.ToString()},
                { nameof(ClubStaffEntity.RoleId), searchRoleId.ToString() }
            };

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity4 = await _clubStaffRepository.CreateAsync(clubStaffEntity4, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, filters: filters, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.ClubId == searchClubId && c.RoleId == searchRoleId));
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity4, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedClubStaff()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() 
            { 
                { nameof(ClubStaffEntity.FirstName), SortDirection.Ascending } 
            };
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithName("Another. This should come first.")
                .Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().FirstName == clubStaffEntity2.FirstName);
                Assert.IsTrue(result.Data.ElementAt(1).FirstName == clubStaffEntity1.FirstName);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedClubStaff()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() 
            { 
                { nameof(ClubStaffEntity.FirstName), SortDirection.Descending } 
            };
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithName("Integration Testing Club")
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithName("Another. This should come last.")
                .Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().FirstName == clubStaffEntity1.FirstName);
                Assert.IsTrue(result.Data.ElementAt(1).FirstName == clubStaffEntity2.FirstName);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedClubStaff()
        {
            // Arrange
            int pageSize = 1;
            int currentPage = 2;
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
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
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
            }
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenClubStaffExist_ShouldReturnClubStaff()
        {
            // Arrange
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            var clubEntityBuilder2 = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = clubEntityBuilder2.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                // Act
                var result = await _clubStaffRepository.GetAllAsync(_cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(0, result.Count());
                Assert.AreEqual(clubStaffEntity1.Id, result.ElementAt(0).Id);
                Assert.AreEqual(clubStaffEntity2.Id, result.ElementAt(1).Id);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1, _cancellationToken);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoClubStaffExist_ShouldReturnEmpty()
        {
            // Act
            var result = await _clubStaffRepository.GetAllAsync(_cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsValid_ShouldCreateAndReturn()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            ClubStaffEntity? result = null;

            try
            {
                // Act
                result = await _clubStaffRepository.CreateAsync(clubStaffEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.ClubId, result.ClubId);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _clubStaffRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffRepository.CreateAsync(clubEntity, _cancellationToken));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsValid_ShouldUpdateAndReturn()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            string editedRoleName = "This is an edited role";
            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                clubStaffEntity.FirstName = editedRoleName;

                // Act
                var result = await _clubStaffRepository.UpdateAsync(clubStaffEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedRoleName, result.FirstName);
            }
            finally
            {
                // Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            // Act
            var result = await _clubStaffRepository.UpdateAsync(clubStaffEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffRepository.UpdateAsync(clubStaffEntity, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffExists_ShouldDeleteAndReturnEntity()
        {
            // Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity, _cancellationToken)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
            }
            finally
            {
                // Act
                var result = await _clubStaffRepository.DeleteAsync(clubStaffEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.Id, result.Id);
                var checkResult = await _clubStaffRepository.GetByIdAsync(clubStaffEntity.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var clubStaff = _clubStaffEntityBuilder.Build();

            // Act
            var result = await _clubStaffRepository.DeleteAsync(clubStaff, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffRepository.DeleteAsync(clubStaffEntity, _cancellationToken));
        }

        #endregion
    }
}
