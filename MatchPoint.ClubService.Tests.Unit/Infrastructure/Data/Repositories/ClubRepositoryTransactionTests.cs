using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Interfaces;
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
        private IClubRepository _repository = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new(_configuration);
            _loggerMock = new();
            _repository = new ClubRepository(_dbContext, _loggerMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext?.Dispose();
        }

        [TestMethod]
        public void IsTransactionActive_WhenNoTransactionIsActive_ShouldReturnFalse()
        {
            // Act
            var result = _repository.IsTransactionActive;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsTransactionActive_WhenTransactionIsActive_ShouldReturnTrue()
        {
            // Arrange - Start a transaction
            _repository.BeginTransaction();

            // Act
            var result = _repository.IsTransactionActive;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenTransactionIsAlreadyActive_ShouldThrowInvalidOperationException()
        {
            // Arrange - Start first transaction
            _repository.BeginTransaction();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(_repository.BeginTransaction);
        }

        [TestMethod]
        public void BeginTransactionAsync_WhenNoTransactionIsActive_ShouldStartTransaction()
        {
            // Act
            _repository.BeginTransaction();

            // Assert - Ensure the transaction is active
            Assert.IsTrue(_repository.IsTransactionActive);
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenNoTransactionIsActive_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _repository.CommitTransactionAsync(_cancellationToken));
        }

        [TestMethod]
        public async Task CommitTransactionAsync_WhenTransactionIsActive_ShouldCommitTransaction()
        {
            // Arrange - Start transaction
            _repository.BeginTransaction();

            // Act
            await _repository.CommitTransactionAsync(_cancellationToken);

            // Assert - Verify no transaction is active
            Assert.IsFalse(_repository.IsTransactionActive);
        }
    }
}
