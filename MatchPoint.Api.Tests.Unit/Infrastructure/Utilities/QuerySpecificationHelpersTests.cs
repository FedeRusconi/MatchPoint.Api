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
            #region Arrange
            int pageNumber = 1;
            int pageSize = 10;
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidatePagination<GenericEntityTest>(pageNumber, pageSize);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Constants.MaxPageSizeAllowed + 1)]
        public void ValidatePagination_WithInvalidPagination_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            #region Act
            var result = QuerySpecificationHelpers.ValidatePagination<GenericEntityTest>(pageNumber, pageSize);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public void ValidateFilters_WithValidFilters_ShouldReturnNull()
        {
            #region Arrange
            Dictionary<string, string> filters = new()
            {
                {nameof(GenericEntityTest.Id), Guid.NewGuid().ToString()}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public void ValidateFilters_WithNullFilters_ShouldReturnNull()
        {
            #region Arrange
            Dictionary<string, string>? filters = null;
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public void ValidateFilters_WithInvalidPropertyFilters_ShouldReturnFailResult()
        {
            #region Arrange
            Dictionary<string, string> filters = new()
            {
                {"Invalid Property", Guid.NewGuid().ToString()}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public void ValidateFilters_WithInvalidValueFilters_ShouldReturnFailResult()
        {
            #region Arrange
            Dictionary<string, string> filters = new()
            {
                {nameof(GenericEntityTest.Id), "Invalid Guid"}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateFilters<GenericEntityTest>(filters);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public void ValidateOrderBy_WithValidOrderBy_ShouldReturnNull()
        {
            #region Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {nameof(GenericEntityTest.Name), SortDirection.Ascending}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public void ValidateOrderBy_WithNullOrderBy_ShouldReturnNull()
        {
            #region Arrange
            Dictionary<string, SortDirection>? orderBy = null;
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);
            #endregion

            #region Assert
            Assert.IsNull(result);
            #endregion
        }

        [TestMethod]
        public void ValidateOrderBy_WithInvalidOrderBy_ShouldReturnFailResult()
        {
            #region Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {"Invalid Property", SortDirection.Descending}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public void ValidateOrderBy_WithMultipleOrderBy_ShouldReturnFailResult()
        {
            #region Arrange
            Dictionary<string, SortDirection> orderBy = new()
            {
                {nameof(GenericEntityTest.Id), SortDirection.Descending},
                {nameof(GenericEntityTest.Name), SortDirection.Descending}
            };
            #endregion

            #region Act
            var result = QuerySpecificationHelpers.ValidateOrderBy<GenericEntityTest>(orderBy);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }
    }
}
