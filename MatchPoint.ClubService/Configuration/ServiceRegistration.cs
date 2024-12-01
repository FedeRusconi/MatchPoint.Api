using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;
using MatchPoint.ClubService.Interfaces;

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
            //services.AddScoped<IPlayerService, PlayerService>();
            #endregion

            return builder;
        }
    }
}
