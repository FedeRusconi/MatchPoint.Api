using MatchPoint.Api.Shared.Common.Enums;

namespace MatchPoint.Api.Shared.Common.Models
{
    public class SocialMediaLink
    {
        public required SocialMedia Platform { get; set; }
        public required string Url { get; set; }
    }
}
