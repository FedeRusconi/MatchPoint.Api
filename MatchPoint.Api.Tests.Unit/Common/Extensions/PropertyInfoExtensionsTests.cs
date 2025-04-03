using System.Reflection;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Common.Extensions
{
    [TestClass]
    public class PropertyInfoExtensionsTests
    {
        [TestMethod]
        public void IsNullable_WhenPropertyIsNullable_ShouldReturnTrue()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            var entityType = patchableEntity.GetType();
            List<PropertyInfo> properties =
            [
                entityType.GetProperty(nameof(PatchableEntityTest.NullableIntProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.NullableDoubleProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.NullableStringProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.NullableDateTimeProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.NullableBoolProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.NullableEnumProperty))!
            ];

            // Act & Assert
            foreach (var property in properties)
            {
                Assert.IsTrue(property.IsNullable());
            }
        }

        [TestMethod]
        public void IsNullable_WhenPropertyIsNotNullable_ShouldReturnFalse()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            var entityType = patchableEntity.GetType();
            List<PropertyInfo> properties =
            [
                entityType.GetProperty(nameof(PatchableEntityTest.IntProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.DoubleProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.StringProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.DateTimeProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.BoolProperty))!,
                entityType.GetProperty(nameof(PatchableEntityTest.EnumProperty))!
            ];

            // Act & Assert
            foreach (var property in properties)
            {
                Assert.IsFalse(property.IsNullable());
            }
        }
    }
}
