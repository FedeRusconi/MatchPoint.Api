using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Interfaces;
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

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _loggerMock = new();
            _clubStaffRepository = new ClubStaffRepository(_dbContext, _loggerMock.Object);
            _clubStaffEntityBuilder = new();
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
            #region Arrange
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity3 = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var resultCount = await _clubStaffRepository.CountAsync();
                #endregion

                #region Assert
                Assert.AreEqual(3, resultCount);
                #endregion
            }
            finally
            {
                #region 
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3);
                #endregion
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredClubStaff()
        {
            #region Arrange
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
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity4 = await _clubStaffRepository.CreateAsync(clubStaffEntity4)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var resultCount = await _clubStaffRepository.CountAsync(filters);
                #endregion

                #region Assert
                Assert.AreEqual(2, resultCount);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity4);
                #endregion
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnClubStaff()
        {
            #region Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetByIdAsync(clubStaffEntity.Id, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.Id, result.Id);
                Assert.AreEqual(clubStaffEntity.ClubId, result.ClubId);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            #region Arrange
            Guid clubStaffId = Guid.NewGuid();
            #endregion

            #region Act
            var result = await _clubStaffRepository.GetByIdAsync(clubStaffId);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredClubStaff()
        {
            #region Arrange
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
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity3 = await _clubStaffRepository.CreateAsync(clubStaffEntity3)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity4 = await _clubStaffRepository.CreateAsync(clubStaffEntity4)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, filters: filters, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.All(c => c.ClubId == searchClubId && c.RoleId == searchRoleId));
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity3);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity4);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedClubStaff()
        {
            #region Arrange
            Dictionary<string, SortDirection> orderBy = new() 
            { 
                { nameof(ClubStaffEntity.RoleName), SortDirection.Ascending } 
            };
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithRoleName("Integration Testing Club")
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithRoleName("Another. This should come first.")
                .Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, orderBy: orderBy, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().RoleName == clubStaffEntity2.RoleName);
                Assert.IsTrue(result.Data.ElementAt(1).RoleName == clubStaffEntity1.RoleName);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedClubStaff()
        {
            #region Arrange
            Dictionary<string, SortDirection> orderBy = new() 
            { 
                { nameof(ClubStaffEntity.RoleName), SortDirection.Descending } 
            };
            var clubStaffEntity1 = _clubStaffEntityBuilder
                .WithRoleName("Integration Testing Club")
                .Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder
                .WithRoleName("Another. This should come last.")
                .Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    1, 10, orderBy: orderBy, trackChanges: false);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.IsTrue(result.Data.First().RoleName == clubStaffEntity1.RoleName);
                Assert.IsTrue(result.Data.ElementAt(1).RoleName == clubStaffEntity2.RoleName);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedClubStaff()
        {
            #region Arrange
            int pageSize = 1;
            int currentPage = 2;
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();

            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetAllWithSpecificationAsync(
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
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                #endregion
            }
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenClubStaffExist_ShouldReturnClubStaff()
        {
            #region Arrange
            var clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            var clubEntityBuilder2 = new ClubStaffEntityBuilder();
            var clubStaffEntity2 = clubEntityBuilder2.Build();

            try
            {
                clubStaffEntity1 = await _clubStaffRepository.CreateAsync(clubStaffEntity1)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                clubStaffEntity2 = await _clubStaffRepository.CreateAsync(clubStaffEntity2)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion

                #region Act
                var result = await _clubStaffRepository.GetAllAsync();
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreNotEqual(0, result.Count());
                Assert.AreEqual(clubStaffEntity1.Id, result.ElementAt(0).Id);
                Assert.AreEqual(clubStaffEntity2.Id, result.ElementAt(1).Id);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity1);
                await _clubStaffRepository.DeleteAsync(clubStaffEntity2);
                #endregion
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenNoClubStaffExist_ShouldReturnEmpty()
        {
            #region Act
            var result = await _clubStaffRepository.GetAllAsync();
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            #endregion
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsValid_ShouldCreateAndReturn()
        {
            #region Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            ClubStaffEntity? result = null;
            #endregion

            try
            {
                #region Act
                result = await _clubStaffRepository.CreateAsync(clubStaffEntity);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.ClubId, result.ClubId);
                Assert.AreNotEqual(default, result.Id);
                #endregion
            }
            finally
            {
                #region Cleanup
                if (result != null)
                {
                    await _clubStaffRepository.DeleteAsync(result);
                }
                #endregion
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubStaffEntity clubEntity = null!;
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubStaffRepository.CreateAsync(clubEntity));
            #endregion
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsValid_ShouldUpdateAndReturn()
        {
            #region Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            string editedRoleName = "This is an edited role";
            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");

                clubStaffEntity.RoleName = editedRoleName;
                #endregion

                #region Act
                var result = await _clubStaffRepository.UpdateAsync(clubStaffEntity);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedRoleName, result.RoleName);
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubStaffRepository.DeleteAsync(clubStaffEntity);
                #endregion
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsNotFound_ShouldReturnNull()
        {
            #region Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();
            #endregion

            #region Act
            var result = await _clubStaffRepository.UpdateAsync(clubStaffEntity);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubStaffEntity clubStaffEntity = null!;
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubStaffRepository.UpdateAsync(clubStaffEntity));
            #endregion
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffExists_ShouldDeleteAndReturnEntity()
        {
            #region Arrange
            var clubStaffEntity = _clubStaffEntityBuilder.Build();

            try
            {
                clubStaffEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity)
                    ?? throw new Exception("_clubStaffRepository.CreateAsync() returned null");
                #endregion
            }
            finally
            {
                #region Act
                var result = await _clubStaffRepository.DeleteAsync(clubStaffEntity);
                #endregion

                #region Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clubStaffEntity.Id, result.Id);
                var checkResult = await _clubStaffRepository.GetByIdAsync(clubStaffEntity.Id);
                Assert.IsNull(checkResult);
                #endregion
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffDoesNotExist_ShouldReturnNull()
        {
            #region
            var clubStaff = _clubStaffEntityBuilder.Build();
            #endregion

            #region Act
            var result = await _clubStaffRepository.DeleteAsync(clubStaff);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubStaffIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubStaffRepository.DeleteAsync(clubStaffEntity));
        }

        #endregion
    }
}
