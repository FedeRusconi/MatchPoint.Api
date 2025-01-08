using MatchPoint.AccessControlService.Infrastructure.Data;

namespace MatchPoint.AccessControlService.Configuration
{
    public static class ServiceRegistration
    {
        public static TBuilder AddAccessControlServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            #region  Data Contexts
            builder.Services.AddDbContext<AccessControlServiceDbContext>();
            #endregion

            #region Repositories
            //builder.Services.AddScoped<IClubRepository, ClubRepository>();
            #endregion

            #region Services
            //builder.Services.AddScoped<IClubManagementService, ClubManagementService>();
            #endregion

            #region Others
            //builder.Services.AddScoped<IAzureAdUserFactory, AzureAdUserFactory>();
            #endregion

            return builder;
        }
    }
}
