using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Tests.Unit.Helpers
{
    internal class DeactivableEntityTest : IDeactivable
    {
        public ActiveStatus ActiveStatus { get; set; }
    }
}
