using MatchPoint.Api.Shared.Enums;

namespace MatchPoint.Api.Shared.Models
{
    public class SocialMediaLink
    {
        public required SocialMedia Platform { get; set; }
        public required string Url { get; set; }
    }
}
