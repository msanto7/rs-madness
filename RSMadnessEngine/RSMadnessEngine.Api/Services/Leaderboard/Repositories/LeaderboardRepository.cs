using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Api.DTOs.Leaderboard;
using RSMadnessEngine.Data;

namespace RSMadnessEngine.Api.Services.Leaderboard.Repositories;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly AppDbContext _dbContext;

    public LeaderboardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LeaderboardEntryResponse>> GetLeaderboardAsync()
    {
        var bracketEntries = await _dbContext.BracketEntries
            .Where(be => be.SubmittedAt != null)
            .Select(be => new
            {
                be.User.DisplayName,
                CurrentPoints = be.Score != null ? be.Score.CurrentPoints : 0,
                PotentialPoints = be.Score != null ? be.Score.PotentialPoints : 0,
                be.SubmittedAt
            })
            .OrderByDescending(be => be.CurrentPoints)
            .ThenByDescending(be => be.PotentialPoints)
            .ToListAsync();

        return bracketEntries
            .Select((be, index) => new LeaderboardEntryResponse
            {
                Position = index + 1,
                UserDisplayName = be.DisplayName ?? string.Empty,
                CurrentPoints = be.CurrentPoints,
                PotentialPoints = be.PotentialPoints,
                SubmittedAt = be.SubmittedAt
            })
            .ToList();
    }
}
