using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Data;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.BracketEntries.Repositories
{
    public class BracketEntryRepository : IBracketEntryRepository
    {
        private readonly AppDbContext _dbContext;

        public BracketEntryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the bracket entry with ranks for the requested user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetBracketEntryResponse?> GetResponseByUserIdAsync(string userId)
        {
            return await _dbContext.BracketEntries
                .Where(be => be.UserId == userId)
                .Select(be => new GetBracketEntryResponse
                {
                    Id = be.Id,
                    SubmittedAt = be.SubmittedAt,
                    Ranks = be.EntryTeamRanks
                        .OrderBy(r => r.Rank)
                        .Select(r => new TeamRankDTO
                        {
                            TeamId = r.TeamId,
                            TeamName = r.Team.Name,
                            Seed = r.Team.Seed,
                            Region = r.Team.Region,
                            Rank = r.Rank
                        }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<BracketEntry?> GetByUserIdWithRanksAsync(string userId)
        {
            return await _dbContext.BracketEntries
                .Include(be => be.EntryTeamRanks)
                .FirstOrDefaultAsync(be => be.UserId == userId);
        }

        /// <summary>
        /// Adds a bracket entry record to the db context.
        /// </summary>
        /// <param name="bracketEntry"></param>
        public void Add(BracketEntry bracketEntry)
        {
            _dbContext.BracketEntries.Add(bracketEntry);
        }

        /// <summary>
        /// Saves all db context changes.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
