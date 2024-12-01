using MatchPoint.Api.Shared.Repositories;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Entities;
using Microsoft.EntityFrameworkCore;
using MatchPoint.Api.Shared.Extensions;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class ClubRepository(ClubServiceDbContext _context) : RepositoryBase(_context), IClubRepository
    {
        /// <inheritdoc />
        public async Task<ClubEntity?> GetByIdAsync(Guid id, bool trackChanges = true)
        {
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClubEntity>> GetAllAsync(bool trackChanges = true)
        {
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ClubEntity> CreateAsync(ClubEntity club)
        {
            ArgumentNullException.ThrowIfNull(club);

            // Detect duplicate
            var existingClub = await _context.Clubs.FirstOrDefaultAsync(c => c.Email == club.Email);
            if (existingClub != null)
            {
                throw new InvalidOperationException("A Club with the same email was found. Operation Canceled.");
            }

            club.SetTrackingFields();
            _context.Clubs.Add(club);
            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return club;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid clubId)
        {
            var club = await _context.Clubs.FindAsync(clubId);
            if (club == null) return false;

            _context.Clubs.Remove(club);

            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
