using System.Reflection;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;

namespace MatchPoint.AccessControlService.Tests.Unit.Mappers
{
    [TestClass]
    public class CustomRoleMapperTests
    {
        private CustomRoleEntityBuilder _customRoleEntityBuilder = default!;
        private CustomRoleBuilder _customRoleBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _customRoleEntityBuilder = new();
            _customRoleBuilder = new();
        }

        #region ToCustomRoleEntity

        [TestMethod]
        public void ToCustomRoleEntity_FromCustomRoleDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            CustomRole customRoleDto = _customRoleBuilder.WithTrackingFields().Build();

            // Act
            CustomRoleEntity result = customRoleDto.ToCustomRoleEntity();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetValue(result) == default)
                {
                    Assert.Fail($"{prop.Name} has not been set.");
                    break;
                }
            }
        }

        [TestMethod]
        public void ToCustomRoleEntity_FromCustomRoleDto_ValidParameter_ShouldReturnCustomRoleEntity()
        {
            // Arrange
            CustomRole customRoleDto = _customRoleBuilder.WithTrackingFields().Build();

            // Act
            CustomRoleEntity result = customRoleDto.ToCustomRoleEntity();

            // Assert
            Assert.AreEqual(customRoleDto.Id, result.Id);
            Assert.AreEqual(customRoleDto.Name, result.Name);
            Assert.IsTrue(result.Capabilities.All(customRoleDto.Capabilities.Contains));
            Assert.AreEqual(customRoleDto.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(customRoleDto.CreatedBy, result.CreatedBy);
            Assert.AreEqual(customRoleDto.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(customRoleDto.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(customRoleDto.ModifiedOnUtc, result.ModifiedOnUtc);
        }

        [TestMethod]
        public void ToCustomRoleEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfCustomRoleEntity()
        {
            // Arrange
            CustomRole customRole1 = _customRoleBuilder.WithTrackingFields().Build();
            _customRoleBuilder = new();
            CustomRole customRole2 = _customRoleBuilder.WithTrackingFields().Build();
            _customRoleBuilder = new();
            CustomRole customRole3 = _customRoleBuilder.WithTrackingFields().Build();
            List<CustomRole> customRoles = [customRole1, customRole2, customRole3];

            // Act
            IEnumerable<CustomRoleEntity> result = customRoles.ToCustomRoleEntityEnumerable();

            // Assert
            Assert.AreEqual(customRoles.Count, result.Count());
            Assert.AreEqual(customRoles[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(customRoles[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(customRoles[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToCustomRoleEntityEnumerable_NullCustomRole_ShouldThrowNullReferenceException()
        {
            // Arrange
            CustomRole customRole1 = _customRoleBuilder.WithTrackingFields().Build();
            CustomRole customRole2 = null!;
            _customRoleBuilder = new();
            CustomRole customRole3 = _customRoleBuilder.WithTrackingFields().Build();
            List<CustomRole> customRoles = [customRole1, customRole2, customRole3];

            // Act & Assert
            var result = customRoles.ToCustomRoleEntityEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
        #region ToCustomRoleDto

        [TestMethod]
        public void ToCustomRoleDto_FromCustomRoleEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = _customRoleEntityBuilder.WithTrackingFields().Build();

            // Act
            CustomRole result = customRoleEntity.ToCustomRoleDto();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetValue(result) == default)
                {
                    Assert.Fail($"{prop.Name} has not been set.");
                    break;
                }
            }
        }

        [TestMethod]
        public void ToCustomRoleDto_FromCustomRoleEntity_ValidParameter_ShouldReturnCustomRoleEntity()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = _customRoleEntityBuilder.WithTrackingFields().Build();

            // Act
            CustomRole result = customRoleEntity.ToCustomRoleDto();

            // Assert
            Assert.AreEqual(customRoleEntity.Id, result.Id);
            Assert.AreEqual(customRoleEntity.Name, result.Name);
            Assert.IsTrue(result.Capabilities.All(customRoleEntity.Capabilities.Contains));
            Assert.AreEqual(customRoleEntity.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(customRoleEntity.CreatedBy, result.CreatedBy);
            Assert.AreEqual(customRoleEntity.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(customRoleEntity.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(customRoleEntity.ModifiedOnUtc, result.ModifiedOnUtc);
        }

        [TestMethod]
        public void ToCustomRoleDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfCustomRoleDto()
        {
            // Arrange
            CustomRoleEntity customRoleEntity1 = _customRoleEntityBuilder.Build();
            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            CustomRoleEntity customRoleEntity2 = _customRoleEntityBuilder.Build();
            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            CustomRoleEntity customRoleEntity3 = _customRoleEntityBuilder.Build();
            List<CustomRoleEntity> customRoleEntities = [customRoleEntity1, customRoleEntity2, customRoleEntity3];

            // Act
            IEnumerable<CustomRole> result = customRoleEntities.ToCustomRoleDtoEnumerable();

            // Assert
            Assert.AreEqual(customRoleEntities.Count, result.Count());
            Assert.AreEqual(customRoleEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(customRoleEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(customRoleEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToCustomRoleDtoEnumerable_NullCustomRoleEntity_ShouldThrowNullReferenceException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity1 = _customRoleEntityBuilder.Build();
            CustomRoleEntity customRoleEntity2 = null!;
            _customRoleEntityBuilder = new CustomRoleEntityBuilder();
            CustomRoleEntity customRoleEntity3 = _customRoleEntityBuilder.Build();
            List<CustomRoleEntity> customRoles = [customRoleEntity1, customRoleEntity2, customRoleEntity3];

            // Act & Assert
            var result = customRoles.ToCustomRoleDtoEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
    }
}
