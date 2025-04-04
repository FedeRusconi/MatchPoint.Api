using Bogus;
using MatchPoint.Api.Shared.ClubService.Models;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class CourtFeatureBuilder
    {
        private readonly List<CourtFeature> _courtFeatures;

        private readonly Dictionary<string, string[]> _sampleFeatures = new() {
            { "Lighting", ["None", "Floodlights", "LED"] },
            { "Location", ["Indoor", "Outdoor"] },
            { "Seating Capacity", ["None", "< 100", "100-1,000", "1,000 - 10,000", "> 10,000"] },
            { "Fencing", ["Open", "Partially Enclosed", "Fully Enclosed"] },
            { "Accessibility", ["No Weelchair Access", "Weelchair Access"] },
            { "Wall", ["None", "Backboard", "Practice Wall"] },
            { "Net Type", ["Permanent", "Portable", "Standard", "Premium"] },
            { "Screens", ["None", "Wind Screens", "Shading"] },
        };

        /// <summary>
        /// Instantiate a new list of <see cref="CourtFeature"/>
        /// </summary>
        public CourtFeatureBuilder()
        {
            _courtFeatures = [];
        }

        /// <summary>
        /// Complete the build operation for a given number of <see cref="CourtFeature"/>.
        /// </summary>
        /// <param name="count"> The number of Features to add. </param>
        /// <returns> The built List of <see cref="CourtFeature"/>. </returns>
        public List<CourtFeature> Build(int count = 3)
        {
            var faker = new Faker();
            var features = faker.Random.ListItems(_sampleFeatures.Keys.ToList(), count < _sampleFeatures.Count ? count : _sampleFeatures.Count);
            foreach (var f in features)
            {
                CourtFeature feature = new()
                {
                    Id = Guid.NewGuid(),
                    Name = f,
                    Value = faker.Random.ListItem(_sampleFeatures[f])
                };
                _courtFeatures.Add(feature);
            }
            return _courtFeatures;
        }
    }
}
