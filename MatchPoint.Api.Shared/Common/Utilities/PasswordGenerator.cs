using System.Security.Cryptography;

namespace MatchPoint.Api.Shared.Common.Utilities
{
    public class PasswordGenerator
    {
        private const string DigitsChars = "0123456789";

        /// <summary>
        /// Generate a random numeric password of the specified length.
        /// </summary>
        /// <param name="digits"> The length of the return pwd. </param>
        /// <param name="prefix"> 
        /// A string to add to the front of the pwd. 
        /// Default is null.
        /// </param>
        public static string GenerateNumeric(int digits, string? prefix = null)
        {
            var pwd = new string(Enumerable.Repeat(DigitsChars, digits)
                .Select(s => s[RandomNumberGenerator.GetInt32(0, s.Length)])
                .ToArray());

            if (!string.IsNullOrEmpty(prefix))
            {
                pwd = prefix + pwd;
            }

            return pwd;
        }
    }
}
