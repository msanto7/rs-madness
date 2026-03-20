using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data;

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

        public async Task SyncGameDataAndRecalculateAsync()
        {
            _logger.LogInformation("Begin syncing...");

            // pull the game result from ESPN

            // load the teams from our DB

            // lookup by name

            // count wins for each team -- and change status of eliminated teams

            // recalculate all the points and potential points for each active bracket entry
        }
    }
}
