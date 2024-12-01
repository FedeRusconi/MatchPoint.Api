using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Tests.Integration.Helpers;
using Microsoft.Extensions.Configuration;

namespace MatchPoint.ClubService.Tests.Integration.Infrastructure.Data.Repositories
{
    [TestClass]
    public class ClubRepositoryTests
    {
        private IClubRepository _clubRepository = null!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private ClubServiceDbContext _dbContext = default!;
        private ClubEntityBuilder _clubEntityBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _dbContext = new ClubServiceDbContext(_configuration);
            _clubRepository = new ClubRepository(_dbContext);
            _clubEntityBuilder = new();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
            _clubRepository = default!;
            _clubEntityBuilder = default!;
        }

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
                var result = await _clubRepository.GetByIdAsync(clubEntity.Id);
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
                Assert.AreNotEqual(default, result.CreatedBy);
                Assert.AreNotEqual(default, result.CreatedOnUtc);
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
        public async Task CreateAsync_WhenClubExistsAlready_ShouldThrowInvalidOperationException()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .WithEmail("duplicate@email.com")
                .Build();

            // Create first club entity
            var result = await _clubRepository.CreateAsync(clubEntity);
            #endregion

            try
            {
                #region Act
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _clubRepository.CreateAsync(clubEntity));
                #endregion
            }
            finally
            {
                #region Cleanup
                await _clubRepository.DeleteAsync(result.Id);
                #endregion
            }
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
                result = await _clubRepository.GetByIdAsync(createResult.Id);
                #endregion

                #region Assert
                Assert.IsNull(result);
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
        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenClubExists_ShouldDeleteAndReturnTrue()
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
                Assert.IsTrue(result);
                var getResult = await _clubRepository.GetByIdAsync(clubEntity.Id);
                Assert.IsNull(getResult);
                #endregion
            }
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubDoesNotExist_ShouldReturnFalse()
        {
            #region
            var clubId = Guid.NewGuid();
            #endregion

            #region Act
            var result = await _clubRepository.DeleteAsync(clubId);
            #endregion

            #region Assert
            Assert.IsFalse(result);
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
                Assert.IsTrue(result);
                var getResult = await _clubRepository.GetByIdAsync(clubEntity.Id);
                Assert.IsNotNull(getResult);
                #endregion

                // Commit to actually delete the test club
                await _clubRepository.CommitTransactionAsync();
            }
        }

        #endregion
    }
}
