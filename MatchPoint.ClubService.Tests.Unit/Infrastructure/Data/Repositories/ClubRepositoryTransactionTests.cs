using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Tests.Unit.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Infrastructure.Data.Repositories
{
    [TestClass]
    public class ClubRepositoryTransactionTests
    {
        private ClubServiceDbContext _dbContext = default!;
        private readonly IConfiguration _configuration = DataContextHelpers.TestingConfiguration;
        private Mock<ILogger<ClubRepository>> _loggerMock = default!;
        private ClubRepository _repository = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new(_configuration);
            _loggerMock = new();
            _repository = new ClubRepository(_dbContext, _loggerMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        [TestMethod]
        public void IsTransactionActive_WhenNoTransactionIsActive_ShouldReturnFalse()
        {
            #region Act
            var result = _repository.IsTransactionActive;
            #endregion

            #region Assert
            Assert.IsFalse(result);
            #endregion
        }

        [TestMethod]
        public void IsTransactionActive_WhenTransactionIsActive_ShouldReturnTrue()
        {
            #region Arrange
            // Start a transaction
            _repository.BeginTransaction();
            #endregion

            #region Act
            var result = _repository.IsTransactionActive;
            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenTransactionIsAlreadyActive_ShouldThrowInvalidOperationException()
        {
            #region Arrange
            // Start first transaction
            _repository.BeginTransaction();
            #endregion

            #region Act & Assert
            Assert.ThrowsException<InvalidOperationException>(_repository.BeginTransaction);
            #endregion
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenNoTransactionIsActive_ShouldStartTransaction()
        {
            #region Act
            _repository.BeginTransaction();
            #endregion

            #region Assert
            // Ensure the transaction is active
            Assert.IsTrue(_repository.IsTransactionActive);
            #endregion
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenNoTransactionIsActive_ShouldThrowInvalidOperationException()
        {
            #region Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(_repository.CommitTransactionAsync);
            #endregion
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenTransactionIsActive_ShouldCommitTransaction()
        {
            #region Arrange
            // Start transaction
            _repository.BeginTransaction();
            #endregion

            #region Act
            await _repository.CommitTransactionAsync();
            #endregion

            #region Assert
            // Verify no transaction is active
            Assert.IsFalse(_repository.IsTransactionActive);
            #endregion
        }
    }
}
