using System.Text.Json;

namespace RSMadnessEngine.Api.Services
{
    public class NcaaDataProvider : INcaaDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NcaaDataProvider> _logger;

        private const string espnApiUrl = "https://site.api.espn.com/apis/site/v2/sports/basketball/mens-college-basketball/scoreboard";

        public NcaaDataProvider(HttpClient httpClient, ILogger<NcaaDataProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Queries the ESPN API to get fresh game data.
        /// </summary>
        /// <returns>List<GameResult> object of winners and losers.</returns>
        public async Task<List<GameResult>> GetTournamentResultsAsync()
        {
            try
            {
                // api endpoint with date filtering
                var dateFilteredUrl = $"{espnApiUrl}?groups=100&limit=200&dates=20260319-20260415";
                _logger.LogInformation("Fetching tournament results from ESPN API: {Url}", dateFilteredUrl);

                // make call to ESPN API endpoint
                var response = await _httpClient.GetAsync(dateFilteredUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);

                var events = jsonDoc.RootElement.GetProperty("events");

                var results = new List<GameResult>();

                // loop the api events
                foreach (var evt in events.EnumerateArray())
                {
                    var competitions = evt.GetProperty("competitions");
                    // loop the api competitions
                    foreach (var comp in competitions.EnumerateArray())
                    {
                        // games need to be completed
                        var status = comp.GetProperty("status").GetProperty("type");
                        if (!status.GetProperty("completed").GetBoolean())
                        {
                            continue;
                        }

                        var competitors = comp.GetProperty("competitors");
                        string? winnerTeamName = null;
                        string? loserTeamName = null;

                        // loop teams in each game
                        foreach (var team in competitors.EnumerateArray())
                        {
                            var teamName = team.GetProperty("team").GetProperty("shortDisplayName").GetString();
                            var isWinner = team.GetProperty("winner").GetBoolean();

                            if (isWinner)
                            {
                                winnerTeamName = teamName;
                            }
                            else
                            {
                                loserTeamName = teamName;
                            }
                        }

                        // add to the results list
                        if (winnerTeamName != null && loserTeamName != null)
                        {
                            results.Add(new GameResult
                            {
                                WinnerTeamName = winnerTeamName!,
                                LoserTeamName = loserTeamName!
                            });
                        }
                    }
                }

                _logger.LogInformation("Successfully fetched tournament results from ESPN API. Total games: {Count}", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tournament results from ESPN API");
                return new List<GameResult>();
            }
        }
    }
}
