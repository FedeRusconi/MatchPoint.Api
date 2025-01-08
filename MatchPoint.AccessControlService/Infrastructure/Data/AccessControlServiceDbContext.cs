using MatchPoint.AccessControlService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.AccessControlService.Infrastructure.Data
{
    public class AccessControlServiceDbContext(IConfiguration _config) : DbContext()
    {
        #region DB Sets
        public DbSet<CustomRoleEntity> CustomRoles { get; set; } = default!;
        public DbSet<ClubRoleEntity> ClubRoles { get; set; } = default!;
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cosmosDbUrl = _config.GetValue<string>("CosmosDb:Url")
                ?? throw new KeyNotFoundException("CosmosDB URL is not defined.");
            var cosmosDbKey = _config.GetValue<string>("CosmosDb:Key")
                ?? throw new KeyNotFoundException("CosmosDB Key is not defined.");
            var databaseName = _config.GetValue<string>("CosmosDb:DatabaseName")
                ?? throw new KeyNotFoundException("CosmosDB Database Name is not defined.");

            optionsBuilder.UseCosmos(
                    cosmosDbUrl,
                    cosmosDbKey,
                    databaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomRoleEntity>().ToContainer(nameof(CustomRoles)).HasPartitionKey(c => c.Id);
            modelBuilder.Entity<ClubRoleEntity>()
                .ToContainer(nameof(ClubRoles))
                .HasPartitionKey(s => s.ClubId)
                .HasKey(e => e.Id);
        }
    }
}
