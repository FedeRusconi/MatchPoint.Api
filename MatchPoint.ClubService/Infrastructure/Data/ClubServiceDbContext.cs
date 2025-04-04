using MatchPoint.ClubService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data
{
    public class ClubServiceDbContext(IConfiguration _config) : DbContext()
    {
        #region DB Sets
        public DbSet<ClubEntity> Clubs { get; set; } = default!;
        public DbSet<ClubStaffEntity> ClubStaff { get; set; } = default!;
        public DbSet<CourtEntity> Courts { get; set; } = default!;
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
            modelBuilder.Entity<ClubEntity>().ToContainer(nameof(Clubs)).HasPartitionKey(c => c.Id);
            modelBuilder.Entity<ClubStaffEntity>()
                .ToContainer(nameof(ClubStaff))
                .HasPartitionKey(s => s.ClubId)
                .HasKey(s => s.Id);
            modelBuilder.Entity<CourtEntity>()
                .ToContainer(nameof(Courts))
                .HasPartitionKey(c => c.ClubId)
                .HasKey(c => c.Id);
        }
    }
}
