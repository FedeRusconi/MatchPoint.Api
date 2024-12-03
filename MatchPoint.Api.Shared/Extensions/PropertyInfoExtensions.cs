using System.Reflection;

namespace MatchPoint.Api.Shared.Extensions
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Checks wheter the provided <see cref="PropertyInfo"/> is of nullble type.
        /// </summary>
        /// <param name="propertyInfo"> The <see cref="PropertyInfo"/> to check. </param>
        /// <returns> True if the property is nullable. </returns>
        public static bool IsNullable(this PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;
            if (propertyType.IsValueType)
            {
                return Nullable.GetUnderlyingType(propertyType) != null;
            }
            else
            {
                var nullableAttribute = propertyInfo.CustomAttributes
                    .FirstOrDefault(attr => attr.AttributeType.Name == "NullableAttribute");

                if (nullableAttribute != null)
                {
                    var nullableFlag = (byte)nullableAttribute.ConstructorArguments[0].Value!;
                    return nullableFlag == 2; // 2 means nullable, 1 means non-nullable
                }
                return false;
            }
        }
    }
}
