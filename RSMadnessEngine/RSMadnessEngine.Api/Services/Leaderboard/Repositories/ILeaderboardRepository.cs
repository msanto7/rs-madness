using RSMadnessEngine.Api.DTOs.Leaderboard;

namespace RSMadnessEngine.Api.Services.Leaderboard.Repositories;

public interface ILeaderboardRepository
{
    Task<List<LeaderboardEntryResponse>> GetLeaderboardAsync();
}
