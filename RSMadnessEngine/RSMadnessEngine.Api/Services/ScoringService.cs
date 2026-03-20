using RSMadnessEngine.Data;
using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services
{
    public class ScoringService : IScoringService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ScoringService> _logger;

        public ScoringService(AppDbContext dbContext, ILogger<ScoringService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Pulls all the submitted bracket entries and recalculates the points.
        /// </summary>
        /// <returns></returns>
        public async Task CalculateScoresAsync()
        {
            // load teams
            var teamStatuses = await _dbContext.TeamStatuses.ToDictionaryAsync(ts => ts.TeamId);

            // load brackets
            var bracketEntries = await _dbContext.BracketEntries
                .Where(be => be.SubmittedAt != null)
                .Include(be => be.EntryTeamRanks)
                .Include(be => be.Score)
                .ToListAsync();

            // loop each bracket
            foreach (var bracketEntry in bracketEntries)
            {
                int currentPoints = 0;
                int potentialPoints = 0;

                // loop through each team entry
                foreach (var rank in bracketEntry.EntryTeamRanks)
                {
                    // calculate points based on status, rank, and wins for each team
                    if (teamStatuses.TryGetValue(rank.TeamId, out var status))
                    {
                        currentPoints += rank.Rank * status.Wins;
                        potentialPoints += (status.IsAlive ? 1 : 0) * (6 - status.Wins) * rank.Rank;
                    }
                    else
                    {
                        potentialPoints += 6 * rank.Rank;
                    }

                }

                // add the bracket score record if one doesn't already exist
                if (bracketEntry.Score == null)
                {
                    bracketEntry.Score = new BracketEntryScore
                    {
                        BracketEntryId = bracketEntry.Id,
                        CurrentPoints = currentPoints,
                        PotentialPoints = potentialPoints
                    };

                    _dbContext.BracketEntryScores.Add(bracketEntry.Score);
                }
                // otherwise just update the existing record
                else
                {
                    bracketEntry.Score.CurrentPoints = currentPoints;
                    bracketEntry.Score.PotentialPoints = potentialPoints;
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Scores calculated and saved successfully.");
        }
    }
}
