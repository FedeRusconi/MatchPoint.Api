using System.Text.Json;

namespace MatchPoint.Api.Shared.Common.Models
{
    public class Address
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required Country Country { get; set; }

        #region Parsing Methods

        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static Address Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Address string cannot be null or empty.");
            }

            try
            {
                return JsonSerializer.Deserialize<Address>(value, serializerOptions)
                    ?? throw new FormatException("Address cannot be null.");
            }
            catch (JsonException ex)
            {
                throw new FormatException("Invalid address format.", ex);
            }
        }

        public static bool TryParse(string value, out Address? address)
        {
            address = null;
            if (string.IsNullOrWhiteSpace(value)) return false;

            try
            {
                address = JsonSerializer.Deserialize<Address>(value, serializerOptions);
                return address != null;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        #endregion

    }
}
