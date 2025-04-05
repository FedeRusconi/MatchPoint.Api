using Bogus;
using MatchPoint.Api.Shared.ClubService.Enums;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class CourtEntityBuilder
    {
        private readonly CourtEntity _courtEntity;

        /// <summary>
        /// Instantiate a random <see cref="CourtEntity"/>
        /// </summary>
        public CourtEntityBuilder()
        {
            var surfaceGenerator = new Faker<Surface>()
                .RuleFor(a => a.Type, f => f.Random.Enum<SurfaceType>())
                .RuleFor(a => a.Material, f => f.Random.Enum<SurfaceMaterial>())
                .RuleFor(a => a.Texture, f => f.Random.Enum<SurfaceTexture>());

            _courtEntity = new Faker<CourtEntity>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.ClubId, Guid.NewGuid)
                .RuleFor(c => c.Name, f => $"Court {f.Random.Int(0, 25)}")
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph(3))
                .RuleFor(c => c.ActiveStatus, ActiveStatus.Active)
                .RuleFor(c => c.Surface, surfaceGenerator.Generate())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithId(Guid id)
        {
            _courtEntity.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithDefaultId()
        {
            _courtEntity.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific ClubId to the <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithClubId(Guid id)
        {
            _courtEntity.ClubId = id;
            return this;
        }

        /// <summary>
        /// Set the default ClubId to the <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithDefaultClubId()
        {
            _courtEntity.ClubId = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithName(string name)
        {
            _courtEntity.Name = name;
            return this;
        }

        /// <summary>
        /// Set the tracking fields (crated, modified) for the <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithTrackingFields()
        {
            var faker = new Faker();
            _courtEntity.CreatedBy = Guid.NewGuid();
            _courtEntity.CreatedOnUtc = faker.Date.Past();
            _courtEntity.ModifiedBy = Guid.NewGuid();
            _courtEntity.ModifiedOnUtc = faker.Date.Past();
            return this;
        }

        /// <summary>
        /// Set the Court Manitenance for the <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithCourtMaintenance()
        {
            var maintenanceGenerator = new Faker<CourtMaintenance>()
                .RuleFor(a => a.NextMaintenance, f => f.Date.Future())
                .RuleFor(a => a.LastMaintenance, f => f.Date.Past())
                .RuleFor(a => a.Frequency, f => new Frequency() { Value = f.Random.Int(1, 7), Unit = f.Random.Enum<PeriodUnit>()})
                .RuleFor(a => a.ReminderFrequency, f => new Frequency() { Value = f.Random.Int(1, 7), Unit = f.Random.Enum<PeriodUnit>()})
                .RuleFor(a => a.Description, f => f.Lorem.Paragraph(1));
            _courtEntity.CourtMaintenance = maintenanceGenerator.Generate();
            return this;
        }

        /// <summary>
        /// Set the Rating for the <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithRating()
        {
            var faker = new Faker();
            _courtEntity.Ratings = [];
            foreach (var rating in Enum.GetValues<CourtRatingAttribute>())
            {
                _courtEntity.Ratings.Add(new CourtRating()
                {
                    Attribute = rating,
                    Value = faker.Random.Int(1, 5)
                });
            }
            return this;
        }

        /// <summary>
        /// Set the Features for the <see cref="CourtEntity"/>.
        /// </summary>
        /// <param name="count"> The number of Features to add. </param>
        /// <returns> This <see cref="CourtEntityBuilder"/>. </returns>
        public CourtEntityBuilder WithFeatures(int count = 3)
        {
            _courtEntity.Features = new CourtFeatureBuilder().Build(count);
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="CourtEntity"/>.
        /// </summary>
        /// <returns> The built <see cref="CourtEntity"/>. </returns>
        public CourtEntity Build()
        {
            return _courtEntity;
        }
    }
}
