using MatchPoint.Api.Shared.Interfaces;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Infrastructure.Data;
using MatchPoint.ClubService.Infrastructure.Data.Repositories;

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
            builder.Services.AddScoped<IRepository<ClubEntity>, ClubRepository>();
            #endregion

            #region Services
            //services.AddScoped<IPlayerService, PlayerService>();
            #endregion

            return builder;
        }
    }
}
