using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Infrastructure.Utilities
{
    [TestClass]
    public class QuerySpecificationHelpersTests
    {
        [TestMethod]
        public void ValidatePagination_WithValidPagination_ShouldReturnNull()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = QuerySpecificationHelpers.ValidatePagination<GenericEntityTest>(pageNumber, pageSize);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Constants.MaxPageSizeAllowed + 1)]
        public void ValidatePagination_WithInvalidPagination_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            // Act
            var result = QuerySpecificationHelpers.ValidatePagination<GenericEntityTest>(pageNumber, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public void ValidateFilters_WithValidFilters_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, string> filters = new()
            {
                {nameof(GenericEntityTest.Id), Guid.NewGuid().ToString()}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateFilters_WithNullFilters_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, string>? filters = null;

            // Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateFilters_WithInvalidPropertyFilters_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, string> filters = new()
            {
                {"Invalid Property", Guid.NewGuid().ToString()}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public void ValidateFilters_WithInvalidValueFilters_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, string> filters = new()
            {
                {nameof(GenericEntityTest.Id), "Invalid Guid"}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public void ValidateOrderBy_WithValidOrderBy_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {nameof(GenericEntityTest.Name), SortDirection.Ascending}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateOrderBy_WithNullOrderBy_ShouldReturnNull()
        {
            // Arrange
            Dictionary<string, SortDirection>? orderBy = null;

            // Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateOrderBy_WithInvalidOrderBy_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {"Invalid Property", SortDirection.Descending}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public void ValidateOrderBy_WithMultipleOrderBy_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {nameof(GenericEntityTest.Id), SortDirection.Descending},
                {nameof(GenericEntityTest.Name), SortDirection.Descending}
            };

            // Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }
    }
}
