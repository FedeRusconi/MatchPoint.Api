using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Common.Extensions
{
    [TestClass]
    public class IPatchableExtensionsTests
    {
        [TestMethod]
        public void Patch_WhenValidTypesAndValuesAreProvided_ShouldUpdateProvidedFields()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            int intProperty = 1;
            int? nullableIntProperty = 2;
            double doubleProperty = 2.5;
            string stringProperty = "This is the edited value";
            string? nullableStringProperty = "This is another edited value";
            DateTime dateTimeProperty = new(2024, 12, 12, 10, 05, 12);
            bool boolProperty = true;
            ActiveStatus enumProperty = ActiveStatus.Active;
            List<PropertyUpdate> propertyUpdates =
            [
                new(nameof(PatchableEntityTest.IntProperty), intProperty),
                new(nameof(PatchableEntityTest.NullableIntProperty), nullableIntProperty),
                new(nameof(PatchableEntityTest.DoubleProperty), doubleProperty),
                new(nameof(PatchableEntityTest.StringProperty), stringProperty),
                new(nameof(PatchableEntityTest.NullableStringProperty), nullableStringProperty),
                new(nameof(PatchableEntityTest.DateTimeProperty), dateTimeProperty),
                new(nameof(PatchableEntityTest.BoolProperty), boolProperty),
                new(nameof(PatchableEntityTest.EnumProperty), enumProperty),
            ];

            // Act
            patchableEntity.Patch(propertyUpdates);

            // Assert
            Assert.AreEqual(intProperty, patchableEntity.IntProperty);
            Assert.AreEqual(nullableStringProperty, patchableEntity.NullableStringProperty);
            Assert.AreEqual(doubleProperty, patchableEntity.DoubleProperty);
            Assert.AreEqual(stringProperty, patchableEntity.StringProperty);
            Assert.AreEqual(nullableStringProperty, patchableEntity.NullableStringProperty);
            Assert.AreEqual(dateTimeProperty, patchableEntity.DateTimeProperty);
            Assert.AreEqual(boolProperty, patchableEntity.BoolProperty);
            Assert.AreEqual(enumProperty, patchableEntity.EnumProperty);
        }

        [TestMethod]
        public void Patch_WhenValidNullValuesAreProvided_ShouldUpdateProvidedFields()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new()
            {
                NullableIntProperty = 1,
                NullableStringProperty = "Not Null"
            };
            List<PropertyUpdate> propertyUpdates =
            [
                new(nameof(PatchableEntityTest.NullableIntProperty), null),
                new(nameof(PatchableEntityTest.NullableStringProperty), null)
            ];

            // Act
            patchableEntity.Patch(propertyUpdates);

            // Assert
            Assert.IsNull(patchableEntity.NullableStringProperty);
            Assert.IsNull(patchableEntity.NullableStringProperty);
        }

        [TestMethod]
        [DataRow(nameof(PatchableEntityTest.IntProperty), null)]
        [DataRow(nameof(PatchableEntityTest.StringProperty), null)]
        public void Patch_WhenInvalidNullValuesAreProvided_ShouldThrowInvalidOperationException(
            string propertyName, string? value)
        {
            // Arrange
            PatchableEntityTest patchableEntity = new()
            {
                IntProperty = 1,
                StringProperty = "Not Nullable"
            };
            List<PropertyUpdate> propertyUpdates =
            [
                new(propertyName, value)
            ];

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }

        [TestMethod]
        public void Patch_WhenPropertyIsReadOnly_ShouldThrowInvalidOperationException()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            List<PropertyUpdate> propertyUpdates =
            [
                new(nameof(PatchableEntityTest.ReadOnlyProperty), 3)
            ];

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }

        [TestMethod]
        [DataRow(nameof(PatchableEntityTest.IntProperty), "Not an int")]
        [DataRow(nameof(PatchableEntityTest.DoubleProperty), "Not a double")]
        [DataRow(nameof(PatchableEntityTest.DateTimeProperty), "Not a datetime")]
        [DataRow(nameof(PatchableEntityTest.BoolProperty), "Not a bool")]
        [DataRow(nameof(PatchableEntityTest.EnumProperty), "Not an enum")]
        public void Patch_WhenInvalidTypesAndValuesAreProvided_ShouldThrowInvalidCastException(
            string propertyName, string value)
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            List<PropertyUpdate> propertyUpdates =
            [
                new(propertyName, value),
            ];

            // Act & Assert
            Assert.ThrowsExactly<InvalidCastException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }

        [TestMethod]
        [DataRow("PrivateProperty", 1)]
        [DataRow(nameof(PatchableEntityTest.StaticProperty), 1)]
        public void Patch_WhenPropertyIsPrivateOrStatic_ShouldThrowArgumentException(
            string propertyName, int value)
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            List<PropertyUpdate> propertyUpdates =
            [
                new(propertyName, value),
            ];

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }

        [TestMethod]
        public void Patch_WhenListOfUpdatesIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            PatchableEntityTest patchableEntity = new();
            List<PropertyUpdate> propertyUpdates = null!;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }

        [TestMethod]
        public void Patch_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            PatchableEntityTest patchableEntity = null!;
            List<PropertyUpdate> propertyUpdates = null!;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _ = patchableEntity.Patch(propertyUpdates));
        }
    }
}
