using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Infrastructure.Data.Repositories;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.AccessControlService.Tests.Integration.Infrastructure.Data.Repositories
{
    [TestClass]
    public class ClubRoleRepositoryTests
    {
        private ClubRoleRepository _clubRoleRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private AccessControlServiceDbContext _dbContext = default!;
        private Mock<ILogger<ClubRoleRepository>> _loggerMock = default!;

        private ClubRoleEntityBuilder _clubRoleEntityBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new AccessControlServiceDbContext(_configuration);
            _loggerMock = new Mock<ILogger<ClubRoleRepository>>();
            _clubRoleRepository = new ClubRoleRepository(_dbContext, _loggerMock.Object);
            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        #region CountAsync

        [TestMethod]
        public async Task CountAsync_WithNoFilters_ShouldReturnCountOfAllClubRoles()
        {
            // Arrange
            var clubRoleEntity1 = _clubRoleEntityBuilder.Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder.Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity3 = _clubRoleEntityBuilder.Build();

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity3 = await _clubRoleRepository.CreateAsync(clubRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubRoleRepository.CountAsync(_cancellationToken);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup 
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredClubRoles()
        {
            // Arrange
            var searchName = "Integration Testing Club Role";
            var clubRoleEntity1 = _clubRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity3 = _clubRoleEntityBuilder
                .WithName("Should not be found")
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity4 = _clubRoleEntityBuilder
                .WithName(searchName)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubRoleEntity.Name), searchName}
            };

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity3 = await _clubRoleRepository.CreateAsync(clubRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity4 = await _clubRoleRepository.CreateAsync(clubRoleEntity4, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _clubRoleRepository.CountAsync(_cancellationToken, filters);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity3, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity4, _cancellationToken);
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnClubRole()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.Build();

            try
            {
                clubRoleEntity = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubRoleEntity.Id, result.Id);
                Assert.AreEqual(clubRoleEntity.Name, result.Name);
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();

            // Act
            var result = await _clubRoleRepository.GetByIdAsync(clubRoleId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenThereAreClubRoles_ShouldReturnAllClubRoles()
        {
            // Arrange
            var clubRoleEntity1 = _clubRoleEntityBuilder.Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder.Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity3 = _clubRoleEntityBuilder.Build();

            Guid[] rolesIds = [clubRoleEntity1.Id, clubRoleEntity2.Id, clubRoleEntity3.Id];

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity3 = await _clubRoleRepository.CreateAsync(clubRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetAllAsync(_cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.IsTrue(result.All(r => rolesIds.Contains(r.Id)));
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenThereAreNoClubRoles_ShouldReturnEmpty()
        {
            // Act
            var result = await _clubRoleRepository.GetAllAsync(_cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredClubRoles()
        {
            // Arrange
            var searchName = "Integration Testing Club Role";
            var clubRoleEntity1 = _clubRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity3 = _clubRoleEntityBuilder
                .WithName("Should not be found")
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(ClubRoleEntity.Name), searchName}
            };

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity3 = await _clubRoleRepository.CreateAsync(clubRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, filters: filters, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.Name == searchName));
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedClubRoles()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(ClubRoleEntity.Name), SortDirection.Ascending } };
            var clubRoleEntity1 = _clubRoleEntityBuilder
                .WithName("Integration Testing Club Role")
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder
                .WithName("Another. This should come first.")
                .Build();

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().Name == clubRoleEntity2.Name);
                Assert.IsTrue(result.Data.ElementAt(1).Name == clubRoleEntity1.Name);
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedClubRoles()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(ClubEntity.Name), SortDirection.Descending } };
            var clubRoleEntity1 = _clubRoleEntityBuilder
                .WithName("Integration Testing Club Role")
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder
                .WithName("Another. This should come last.")
                .Build();

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(clubRoleEntity1.Name, result.Data.First().Name);
                Assert.AreEqual(clubRoleEntity2.Name, result.Data.ElementAt(1).Name);
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedClubRoles()
        {
            // Arrange
            int pageSize = 1;
            int currentPage = 2;
            var clubRoleEntity1 = _clubRoleEntityBuilder
                .WithName("Integration Testing Club Role")
                .Build();

            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            var clubRoleEntity2 = _clubRoleEntityBuilder
                .WithName("Integration Testing Club Role")
                .Build();

            try
            {
                clubRoleEntity1 = await _clubRoleRepository.CreateAsync(clubRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
                clubRoleEntity2 = await _clubRoleRepository.CreateAsync(clubRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.GetAllWithSpecificationAsync(
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
                await _clubRoleRepository.DeleteAsync(clubRoleEntity1, _cancellationToken);
                await _clubRoleRepository.DeleteAsync(clubRoleEntity2, _cancellationToken);
            }
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIsValid_ShouldCreateAndReturn()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithName("Integration Tests Club Role").Build();
            ClubRoleEntity? result = null;

            try
            {
                // Act
                result = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubRoleEntity.Name, result.Name);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _clubRoleRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIdIsDuplicate_ShouldReturnNull()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = _clubRoleEntityBuilder.Build();

            try
            {
                // Add it first to mimic the Id duplicate in the Act part;
                clubRoleEntity = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNull(result);
            }
            finally
            {
                // Cleanup
                _dbContext.Entry(clubRoleEntity).State = EntityState.Detached;
                await _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsValid_ShouldUpdateAndReturn()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithName("Integration Testing Club Role").Build();
            string editedName = "This is an edited role";
            try
            {
                clubRoleEntity = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");

                clubRoleEntity.Name = editedName;

                // Act
                var result = await _clubRoleRepository.UpdateAsync(clubRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedName, result.Name);
            }
            finally
            {
                // Cleanup
                await _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.Build();

            // Act
            var result = await _clubRoleRepository.UpdateAsync(clubRoleEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleRepository.UpdateAsync(clubRoleEntity, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubRoleExists_ShouldDeleteAndReturnEntity()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.Build();

            try
            {
                clubRoleEntity = await _clubRoleRepository.CreateAsync(clubRoleEntity, _cancellationToken)
                    ?? throw new Exception("_clubRoleRepository.CreateAsync() returned null");
            }
            finally
            {
                // Act
                var result = await _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubRoleEntity.Id, result.Id);
                var checkResult = await _clubRoleRepository.GetByIdAsync(clubRoleEntity.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubRoleDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.Build();

            // Act
            var result = await _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleRepository.DeleteAsync(clubRoleEntity, _cancellationToken));
        }

        #endregion
    }
}
