using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Infrastructure.Data.Repositories;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.AccessControlService.Tests.Integration.Infrastructure.Data.Repositories
{
    [TestClass]
    public class CustomRoleRepositoryTests
    {
        private CustomRoleRepository _customRoleRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private AccessControlServiceDbContext _dbContext = default!;
        private Mock<ILogger<CustomRoleRepository>> _loggerMock = default!;

        private CustomRoleEntityBuilder _customRoleEntityBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new AccessControlServiceDbContext(_configuration);
            _loggerMock = new Mock<ILogger<CustomRoleRepository>>();
            _customRoleRepository = new CustomRoleRepository(_dbContext, _loggerMock.Object);
            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        #region CountAsync

        [TestMethod]
        public async Task CountAsync_WithNoFilters_ShouldReturnCountOfAllCustomRoles()
        {
            // Arrange
            var customRoleEntity1 = _customRoleEntityBuilder.Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder.Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity3 = _customRoleEntityBuilder.Build();

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity3 = await _customRoleRepository.CreateAsync(customRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _customRoleRepository.CountAsync(_cancellationToken);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup 
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task CountAsync_WithValidFilters_ShouldReturnCountOfFilteredCustomRoles()
        {
            // Arrange
            var searchName = "Integration Testing Role";
            var customRoleEntity1 = _customRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity3 = _customRoleEntityBuilder
                .WithName("Should not be found")
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity4 = _customRoleEntityBuilder
                .WithName(searchName)
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(CustomRoleEntity.Name), searchName}
            };

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity3 = await _customRoleRepository.CreateAsync(customRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity4 = await _customRoleRepository.CreateAsync(customRoleEntity4, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var resultCount = await _customRoleRepository.CountAsync(_cancellationToken, filters);

                // Assert
                Assert.AreEqual(3, resultCount);
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity3, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity4, _cancellationToken);
            }
        }

        #endregion

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnCustomRole()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            try
            {
                customRoleEntity = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetByIdAsync(customRoleEntity.Id, _cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(customRoleEntity.Id, result.Id);
                Assert.AreEqual(customRoleEntity.Name, result.Name);
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();

            // Act
            var result = await _customRoleRepository.GetByIdAsync(customRoleId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenThereAreCustomRoles_ShouldReturnAllCustomRoles()
        {
            // Arrange
            var customRoleEntity1 = _customRoleEntityBuilder.Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder.Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity3 = _customRoleEntityBuilder.Build();

            Guid[] rolesIds = [customRoleEntity1.Id, customRoleEntity2.Id, customRoleEntity3.Id];

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity3 = await _customRoleRepository.CreateAsync(customRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetAllAsync(_cancellationToken, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.IsTrue(result.All(r => rolesIds.Contains(r.Id)));
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllAsync_WhenThereAreNoCustomRoles_ShouldReturnEmpty()
        {
            // Act
            var result = await _customRoleRepository.GetAllAsync(_cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidFilters_ShouldReturnFilteredCustomRoles()
        {
            // Arrange
            var searchName = "Integration Testing Custom Role";
            var customRoleEntity1 = _customRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder
                .WithName(searchName)
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity3 = _customRoleEntityBuilder
                .WithName("Should not be found")
                .Build();

            Dictionary<string, string> filters = new()
            {
                { nameof(CustomRoleEntity.Name), searchName}
            };

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity3 = await _customRoleRepository.CreateAsync(customRoleEntity3, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetAllWithSpecificationAsync(
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
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity3, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingAscending_ShouldReturnOrderedCustomRoles()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(CustomRoleEntity.Name), SortDirection.Ascending } };
            var customRoleEntity1 = _customRoleEntityBuilder
                .WithName("Integration Testing Custom Role")
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder
                .WithName("Another. This should come first.")
            .Build();

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(customRoleEntity2.Name, result.Data.First().Name);
                Assert.AreEqual(customRoleEntity1.Name, result.Data.ElementAt(1).Name);
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidOrderingDescending_ShouldReturnOrderedCustomRoles()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new() { { nameof(CustomRoleEntity.Name), SortDirection.Descending } };
            var customRoleEntity1 = _customRoleEntityBuilder
                .WithName("Integration Testing Custom Role")
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder
                .WithName("Another. This should come last.")
            .Build();

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetAllWithSpecificationAsync(
                    1, 10, _cancellationToken, orderBy: orderBy, trackChanges: false);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Data.Count());
                Assert.AreEqual(2, result.TotalCount);
                Assert.AreEqual(customRoleEntity1.Name, result.Data.First().Name);
                Assert.AreEqual(customRoleEntity2.Name, result.Data.ElementAt(1).Name);
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WithValidPaging_ShouldReturnPagedCustomRoles()
        {
            // Arrange
            int pageSize = 1;
            int currentPage = 2;
            var customRoleEntity1 = _customRoleEntityBuilder
                .WithName("Integration Testing Custom Role")
                .Build();

            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            var customRoleEntity2 = _customRoleEntityBuilder
                .WithName("Integration Testing Custom Role")
            .Build();

            try
            {
                customRoleEntity1 = await _customRoleRepository.CreateAsync(customRoleEntity1, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
                customRoleEntity2 = await _customRoleRepository.CreateAsync(customRoleEntity2, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.GetAllWithSpecificationAsync(
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
                await _customRoleRepository.DeleteAsync(customRoleEntity1, _cancellationToken);
                await _customRoleRepository.DeleteAsync(customRoleEntity2, _cancellationToken);
            }
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIsValid_ShouldCreateAndReturn()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.WithName("Integration Tests Role").Build();
            CustomRoleEntity? result = null;

            try
            {
                // Act
                result = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(customRoleEntity.Name, result.Name);
                Assert.AreNotEqual(default, result.Id);
            }
            finally
            {
                // Cleanup
                if (result != null)
                {
                    await _customRoleRepository.DeleteAsync(result, _cancellationToken);
                }
            }
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIdIsDuplicate_ShouldReturnNull()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = _customRoleEntityBuilder.Build();

            try
            {
                // Add it first to mimic the Id duplicate in the Act part;
                customRoleEntity = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                // Act
                var result = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNull(result);
            }
            finally
            {
                // Cleanup
                _dbContext.Entry(customRoleEntity).State = EntityState.Detached;
                await _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsValid_ShouldUpdateAndReturn()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.WithName("Integration Testing Role").Build();
            string editedName = "This is an edited role";
            try
            {
                customRoleEntity = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");

                customRoleEntity.Name = editedName;

                // Act
                var result = await _customRoleRepository.UpdateAsync(customRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(editedName, result.Name);
            }
            finally
            {
                // Cleanup
                await _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            // Act
            var result = await _customRoleRepository.UpdateAsync(customRoleEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = null!;

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleRepository.UpdateAsync(customRoleEntity, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenCustomRoleExists_ShouldDeleteAndReturnEntity()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            try
            {
                customRoleEntity = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken)
                    ?? throw new Exception("_customRoleRepository.CreateAsync() returned null");
            }
            finally
            {
                // Act
                var result = await _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(customRoleEntity.Id, result.Id);
                var checkResult = await _customRoleRepository.GetByIdAsync(customRoleEntity.Id, _cancellationToken);
                Assert.IsNull(checkResult);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCustomRoleDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            // Act
            var result = await _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCustomRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleRepository.DeleteAsync(customRoleEntity, _cancellationToken));
        }

        #endregion
    }
}
