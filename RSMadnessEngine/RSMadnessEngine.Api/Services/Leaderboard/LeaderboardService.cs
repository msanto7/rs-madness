using RSMadnessEngine.Api.DTOs.Leaderboard;
using RSMadnessEngine.Api.Services.Leaderboard.Repositories;

namespace RSMadnessEngine.Api.Services.Leaderboard;

public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderboardRepository;

    public LeaderboardService(ILeaderboardRepository leaderboardRepository)
    {
        _leaderboardRepository = leaderboardRepository;
    }

    public Task<List<LeaderboardEntryResponse>> GetLeaderboardAsync()
    {
        return _leaderboardRepository.GetLeaderboardAsync();
    }
}
