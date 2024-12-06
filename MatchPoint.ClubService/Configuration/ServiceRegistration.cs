using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Services;

namespace MatchPoint.ClubService.Configuration
{
    public static class ServiceRegistration
    {
        public static TBuilder AddClubServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            #region  Data Contexts
            builder.Services.AddDbContext<ClubServiceDbContext>();
            #endregion

            #region Repositories
            builder.Services.AddScoped<IClubRepository, ClubRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IClubManagementService, ClubManagementService>();
            #endregion

            return builder;
        }
    }
}
