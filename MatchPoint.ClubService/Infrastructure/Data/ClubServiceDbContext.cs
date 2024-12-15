using MatchPoint.ClubService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data
{
    public class ClubServiceDbContext(IConfiguration _config) : DbContext()
    {
        #region DB Sets
        public DbSet<ClubEntity> Clubs { get; set; } = default!;
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cosmosDbUrl = _config.GetValue<string>("CosmosDb:Url");
            var cosmosDbKey = _config.GetValue<string>("CosmosDb:Key");
            var databaseName = _config.GetValue<string>("CosmosDb:DatabaseName");
            Console.WriteLine($"Using database: {databaseName}");
            if (cosmosDbUrl != null && cosmosDbKey != null && databaseName != null)
            {
                optionsBuilder.UseCosmos(
                    cosmosDbUrl,
                    cosmosDbKey,
                    databaseName);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClubEntity>().ToContainer("Clubs");
        }
    }
}
