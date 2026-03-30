using RSMadnessEngine.Api.DTOs.Leaderboard;

namespace RSMadnessEngine.Api.Services.Leaderboard;

public interface ILeaderboardService
{
    Task<List<LeaderboardEntryResponse>> GetLeaderboardAsync();
}
