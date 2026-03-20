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
                var url = $"{espnApiUrl}?groups=100&limit=200&dates=20260301-20260415";
                _logger.LogInformation("Fetching tournament results from ESPN API: {Url}", url);

                // make call to ESPN API endpoint
                var response = await _httpClient.GetAsync(espnApiUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);

                var results = new List<GameResult>();


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
