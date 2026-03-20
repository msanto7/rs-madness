namespace RSMadnessEngine.Api.Services
{
    public interface INcaaDataProvider
    {
        Task<List<GameResult>> GetTournamentResultsAsync();
    }

    public class GameResult
    {
        public string WinnerTeamName { get; set; } = string.Empty;
        public string LoserTeamName { get; set; } = string.Empty;
    }
}
