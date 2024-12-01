using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Tests.Unit.Helpers;
using Microsoft.Extensions.Configuration;

namespace MatchPoint.ClubService.Tests.Unit.Infrastructure.Data.Repositories
{
    [TestClass]
    public class ClubRepositoryTransactionTests
    {
        private ClubServiceDbContext _dbContext = default!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new(_configuration);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        [TestMethod]
        public void IsTransactionActive_WhenNoTransactionIsActive_ShouldReturnFalse()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);
            #endregion

            #region Act
            var result = repository.IsTransactionActive;
            #endregion

            #region Assert
            Assert.IsFalse(result);
            #endregion
        }

        [TestMethod]
        public void IsTransactionActive_WhenTransactionIsActive_ShouldReturnTrue()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);
            // Start a transaction
            repository.BeginTransaction();
            #endregion

            #region Act
            var result = repository.IsTransactionActive;
            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenTransactionIsAlreadyActive_ShouldThrowInvalidOperationException()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);
            // Start first transaction
            repository.BeginTransaction();
            #endregion

            #region Act & Assert
            Assert.ThrowsException<InvalidOperationException>(repository.BeginTransaction);
            #endregion
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenNoTransactionIsActive_ShouldStartTransaction()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);
            #endregion

            #region Act
            repository.BeginTransaction();
            #endregion

            #region Assert
            // Ensure the transaction is active
            Assert.IsTrue(repository.IsTransactionActive);
            #endregion
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenNoTransactionIsActive_ShouldThrowInvalidOperationException()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(repository.CommitTransactionAsync);
            #endregion
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenTransactionIsActive_ShouldCommitTransaction()
        {
            #region Arrange
            var repository = new ClubRepository(_dbContext);

            // Start transaction
            repository.BeginTransaction();
            #endregion

            #region Act
            await repository.CommitTransactionAsync();
            #endregion

            #region Assert
            // Verify no transaction is active
            Assert.IsFalse(repository.IsTransactionActive);
            #endregion
        }
    }
}
