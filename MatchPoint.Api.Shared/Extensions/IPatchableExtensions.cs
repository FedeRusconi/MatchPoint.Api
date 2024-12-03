using MatchPoint.Api.Shared.Interfaces;
using MatchPoint.Api.Shared.Models;
using System.Reflection;

namespace MatchPoint.Api.Shared.Extensions
{
    public static class IPatchableExtensions
    {
        /// <summary>
        /// Update the fields defined within the enumerable of <see cref="PropertyUpdate"/>
        /// of the entity provided. The entity must implement <see cref="IPatchable"/>.
        /// </summary>
        /// <param name="entity"> The entity to update. </param>
        /// <param name="updates"> An enumerable of <see cref="PropertyUpdate"/> that contains the fields to update. </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static IPatchable Patch(this IPatchable entity, IEnumerable<PropertyUpdate> updates)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            ArgumentNullException.ThrowIfNull(updates, nameof(updates));

            // Get the type of the entity
            var entityType = entity.GetType();

            // Iterate through each PatchInfo item
            foreach (var patchProperty in updates)
            {
                // Get the property from the entity that matches the property name
                var propertyInfo = entityType.GetProperty(patchProperty.Property, BindingFlags.Public | BindingFlags.Instance)
                    ?? throw new ArgumentException($"Property '{patchProperty.Property}' not found on '{entityType.Name}'");

                if (patchProperty.Value == null)
                {
                    // Check to see if the property is not nullable 
                    if (!propertyInfo.IsNullable())
                    {
                        throw new InvalidOperationException($"Non-nullable property {patchProperty.Property} received as null and cannot be updated.");
                    }

                    propertyInfo.SetValue(entity, patchProperty.Value);
                    continue;
                }

                // Check if the property can be written to (is not read-only)
                if (!propertyInfo.CanWrite)
                {
                    throw new InvalidOperationException($"Property '{patchProperty.Property}' on '{entityType.Name}' is read-only");
                }

                // Find expected type (the type of the property to edit)
                var expectedType = propertyInfo.PropertyType;
                expectedType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;

                // Convert incoming value to string or throw exception if not possible
                var patchValueString = patchProperty.Value.ToString()
                    ?? throw new InvalidCastException($"{patchProperty.Value} cannot be converted to {expectedType}.");

                // If expected type is string, simply assign the value
                if (expectedType == typeof(string))
                {
                    propertyInfo.SetValue(entity, patchValueString);
                }
                // if expected type is an enum, convert and assign.
                else if (expectedType.IsEnum)
                {
                    // Handle enums
                    if (Enum.TryParse(expectedType, patchValueString, out var enumValue))
                    {
                        propertyInfo.SetValue(entity, enumValue);
                    }
                    else
                    {
                        throw new InvalidCastException($"{patchProperty.Value} cannot be converted to {expectedType}.");
                    }
                }
                // For all other types, dynamically get the "TryParse" method and convert the value
                else
                {
                    var tryParseMethod = expectedType.GetMethod("TryParse", [typeof(string), expectedType.MakeByRefType()])
                        ?? throw new InvalidCastException($"{expectedType} does not have a TryParse method.");
                    var expectedTypeInstance = Activator.CreateInstance(expectedType)
                        ?? throw new InvalidCastException($"Cannot create an instance of {expectedType}.");
                    var parameters = new object[] { patchValueString, expectedTypeInstance };
                    bool? success = (bool?)tryParseMethod.Invoke(null, parameters);
                    if (success != true)
                    {
                        throw new InvalidCastException($"{patchProperty.Value} cannot be converted to {expectedType}.");
                    }
                    propertyInfo.SetValue(entity, parameters[1]);
                }
            }

            return entity;
        }
    }
}
