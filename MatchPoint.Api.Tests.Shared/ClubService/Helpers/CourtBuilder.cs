using Bogus;
using MatchPoint.Api.Shared.ClubService.Enums;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class CourtBuilder
    {
        private readonly Court _court;

        /// <summary>
        /// Instantiate a random <see cref="Court"/>
        /// </summary>
        public CourtBuilder()
        {
            var surfaceGenerator = new Faker<Surface>()
                .RuleFor(a => a.Type, f => f.Random.Enum<SurfaceType>())
                .RuleFor(a => a.Material, f => f.Random.Enum<SurfaceMaterial>())
                .RuleFor(a => a.Texture, f => f.Random.Enum<SurfaceTexture>());

            _court = new Faker<Court>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.ClubId, Guid.NewGuid)
                .RuleFor(c => c.Name, f => $"Court {f.Random.Int(0, 25)}")
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph(3))
                .RuleFor(c => c.ActiveStatus, ActiveStatus.Active)
                .RuleFor(c => c.Surface, surfaceGenerator.Generate())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="Court"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithId(Guid id)
        {
            _court.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="Court"/>.
        /// </summary>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithDefaultId()
        {
            _court.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific ClubId to the <see cref="Court"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithClubId(Guid id)
        {
            _court.ClubId = id;
            return this;
        }

        /// <summary>
        /// Set the default ClubId to the <see cref="Court"/>.
        /// </summary>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithDefaultClubId()
        {
            _court.ClubId = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="Court"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithName(string name)
        {
            _court.Name = name;
            return this;
        }

        /// <summary>
        /// Set the tracking fields (crated, modified) for the <see cref="Court"/>.
        /// </summary>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithTrackingFields()
        {
            var faker = new Faker();
            _court.CreatedBy = Guid.NewGuid();
            _court.CreatedOnUtc = faker.Date.Past();
            _court.ModifiedBy = Guid.NewGuid();
            _court.ModifiedOnUtc = faker.Date.Past();
            return this;
        }

        /// <summary>
        /// Set the Court Manitenance for the <see cref="Court"/>.
        /// </summary>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithCourtMaintenance()
        {
            var maintenanceGenerator = new Faker<CourtMaintenance>()
                .RuleFor(a => a.NextMaintenance, f => f.Date.Future())
                .RuleFor(a => a.LastMaintenance, f => f.Date.Past())
                .RuleFor(a => a.Frequency, f => new Frequency() { Value = f.Random.Int(1, 7), Unit = f.Random.Enum<PeriodUnit>() })
                .RuleFor(a => a.ReminderFrequency, f => new Frequency() { Value = f.Random.Int(1, 7), Unit = f.Random.Enum<PeriodUnit>() })
                .RuleFor(a => a.Description, f => f.Lorem.Paragraph(1));
            _court.CourtMaintenance = maintenanceGenerator.Generate();
            return this;
        }

        /// <summary>
        /// Set the Rating for the <see cref="Court"/>.
        /// </summary>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithRating()
        {
            var faker = new Faker();
            _court.Ratings = [];
            foreach (var rating in Enum.GetValues<CourtRatingAttribute>())
            {
                _court.Ratings.Add(new CourtRating()
                {
                    Attribute = rating,
                    Value = faker.Random.Int(1, 5)
                });
            }
            return this;
        }

        /// <summary>
        /// Set the Features for the <see cref="Court"/>.
        /// </summary>
        /// <param name="count"> The number of Features to add. </param>
        /// <returns> This <see cref="CourtBuilder"/>. </returns>
        public CourtBuilder WithFeatures(int count = 3)
        {
            _court.Features = new CourtFeatureBuilder().Build(count);
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="Court"/>.
        /// </summary>
        /// <returns> The built <see cref="Court"/>. </returns>
        public Court Build()
        {
            return _court;
        }
    }
}
