using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services
{
    public class SyncService : ISyncService
    {
        private readonly AppDbContext _dbContext;
        private readonly INcaaDataProvider _ncaaDataProvider;
        private readonly IScoringService _scoringService;
        private readonly ILogger<SyncService> _logger;

        public SyncService(AppDbContext dbContext, INcaaDataProvider ncaaDataProvider, IScoringService scoringService, ILogger<SyncService> logger)
        {
            _dbContext = dbContext;
            _ncaaDataProvider = ncaaDataProvider;
            _scoringService = scoringService;
            _logger = logger;
        }

        /// <summary>
        /// Service to combine the pulled ESPN game data and the scoring updates for each bracket.
        /// </summary>
        /// <returns></returns>
        public async Task SyncGameDataAndRecalculateAsync()
        {
            _logger.LogInformation("Begin syncing...");

            // pull the game result from ESPN
            var gameResults = await _ncaaDataProvider.GetTournamentResultsAsync();

            if (gameResults.Count == 0)
            {
                _logger.LogInformation("No completed games found in ESPN data. Skipping sync.");
                return;
            }

            // load the teams from our DB
            var teams = await _dbContext.Teams.Include(t => t.TeamStatus).ToListAsync();

            // count wins for each team -- and change status of eliminated teams
            var wins = new Dictionary<int, int>();
            var eliminated = new HashSet<int>();

            foreach (var game in gameResults)
            {
                var winningTeam = teams.Where(x => x.Name == game.WinnerTeamName).First();
                wins[winningTeam.Id] = wins.GetValueOrDefault(winningTeam.Id) + 1;

                var losingTeam = teams.Where(x => x.Name == game.LoserTeamName).First();
                eliminated.Add(losingTeam.Id);
            }

            // update wins and elimination status in the db
            foreach (var team in teams)
            {
                var teamWins = wins.GetValueOrDefault(team.Id, 0);
                var isAlive = !eliminated.Contains(team.Id);

                if (team.TeamStatus == null)
                {
                    // create new status record if none exists
                    team.TeamStatus = new Data.Entities.TeamStatus
                    {
                        TeamId = team.Id,
                        Wins = teamWins,
                        IsAlive = isAlive,
                    };
                    _dbContext.TeamStatuses.Add(team.TeamStatus);
                }
                else
                {
                    team.TeamStatus.Wins = teamWins;
                    team.TeamStatus.IsAlive = isAlive;
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("TeamStatus updated.");

            // recalculate all the points and potential points for each active bracket entry
            await _scoringService.CalculateScoresAsync();

            _logger.LogInformation("Tournament sync complete.");
        }
    }
}
