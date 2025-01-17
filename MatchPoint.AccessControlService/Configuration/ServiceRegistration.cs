using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Infrastructure.Data;
using MatchPoint.AccessControlService.Infrastructure.Data.Repositories;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Services;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

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
            builder.Services.AddScoped<IRepository<CustomRoleEntity>, CustomRoleRepository>();
            builder.Services.AddScoped<IClubRoleRepository, ClubRoleRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<ICustomRoleService, CustomRoleService>();
            builder.Services.AddScoped<IClubRoleService, ClubRoleService>();
            #endregion

            #region Others
            //builder.Services.AddScoped<IAzureAdUserFactory, AzureAdUserFactory>();
            #endregion

            return builder;
        }
    }
}
