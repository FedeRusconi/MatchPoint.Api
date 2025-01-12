using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Infrastructure.Data.Repositories;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Entities;
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
            var searchName = "Integration Testing Club";
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
            var result = await _clubRepository.GetByIdAsync(customRoleId, _cancellationToken);

            // Assert
            Assert.IsNull(result);
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
            CustomRoleEntity? result = null;

            try
            {
                // Add it first to mimic the Id duplicate in the Act part
                await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken);

                // Act
                result = await _customRoleRepository.CreateAsync(customRoleEntity, _cancellationToken);

                // Assert
                Assert.IsNull(result);
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

        #endregion
    }
}
