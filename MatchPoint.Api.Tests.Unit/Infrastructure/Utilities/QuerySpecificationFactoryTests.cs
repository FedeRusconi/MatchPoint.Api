using System.Linq.Expressions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Infrastructure.Utilities
{
    [TestClass]
    public class QuerySpecificationFactoryTests
    {
        #region CreateFilters

        [TestMethod]
        public void CreateFilters_WithValidFilters_ShouldReturnFilterExpression()
        {
            #region Arrange
            string idPropName = nameof(GenericEntityTest.Id);
            Guid idPropValue = Guid.NewGuid();
            string descPropName = nameof(GenericEntityTest.Description);
            string createdPropName = nameof(GenericEntityTest.CreatedOn);
            DateTime createdPropValue = new(2024, 12, 20);
            string availablePropName = nameof(GenericEntityTest.Available);
            int availablePropValue = 123;
            Dictionary<string, string> filters = new()
            {
                { idPropName, idPropValue.ToString() },
                { descPropName, "Test Description" },
                { createdPropName, createdPropValue.ToString() },
                { availablePropName, availablePropValue.ToString() }
            };
            var parameter = Expression.Parameter(typeof(GenericEntityTest), "entity");
            var propertyId = Expression.Property(parameter, idPropName);
            var constantId = Expression.Constant(idPropValue);
            var propertyDesc = Expression.Property(parameter, descPropName);
            var constantDesc = Expression.Constant(filters[descPropName]);
            var propertyCreated = Expression.Property(parameter, createdPropName);
            var constantCreated = Expression.Constant(createdPropValue);
            var propertyAvailable = Expression.Property(parameter, availablePropName);
            var constantAvailable = Expression.Constant(availablePropValue);
            var expectedExp = Expression.Lambda<Func<GenericEntityTest, bool>>(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.AndAlso(
                            Expression.Equal(propertyId, constantId),
                            Expression.Equal(propertyDesc, constantDesc)),
                        Expression.Equal(propertyCreated, constantCreated)),
                    Expression.Equal(propertyAvailable, constantAvailable)),
                parameter);
            #endregion

            #region Act
            var filterExp = QuerySpecificationFactory<GenericEntityTest>.CreateFilters(filters);
            #endregion

            #region Assert
            Assert.IsNotNull(filterExp);
            Assert.IsTrue(ExpressionComparer.Compare(expectedExp, filterExp));
            #endregion
        }

        [TestMethod]
        public void CreateFilters_WithEmptyFilters_ShouldThrowArgumentException()
        {
            #region Arrange
            Dictionary<string, string> filters = [];
            #endregion

            #region Act & Assert
            Assert.ThrowsException<ArgumentException>(
                () => QuerySpecificationFactory<GenericEntityTest>.CreateFilters(filters));
            #endregion
        }

        #endregion
        #region CreateOrderBy

        [TestMethod]
        public void CreateOrderBy_WithValidPropertyName_ShouldReturnOrderByExpression()
        {
            #region Arrange
            string idPropName = nameof(GenericEntityTest.Id);

            var parameter = Expression.Parameter(typeof(GenericEntityTest), "entity");
            var propertyExpression = Expression.Property(parameter, idPropName);
            var expectedExp = Expression.Lambda<Func<GenericEntityTest, object>>(
                Expression.Convert(propertyExpression, typeof(object)), parameter);
            #endregion

            #region Act
            var orderByExp = QuerySpecificationFactory<GenericEntityTest>.CreateOrderBy(idPropName);
            #endregion

            #region Assert
            Assert.IsNotNull(orderByExp);
            Assert.IsTrue(ExpressionComparer.Compare(expectedExp, orderByExp));
            #endregion
        }

        [TestMethod]
        public void CreateOrderBy_WithNullPropertyName_ShouldThrowArgumentNullException()
        {
            #region Arrange
            string idPropName = null!;
            #endregion

            #region Act & Assert
            Assert.ThrowsException<ArgumentNullException>(
                () => QuerySpecificationFactory<GenericEntityTest>.CreateOrderBy(idPropName));
            #endregion
        }

        #endregion
    }
}