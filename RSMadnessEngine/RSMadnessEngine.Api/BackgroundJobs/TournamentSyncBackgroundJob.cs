using RSMadnessEngine.Api.Services;

namespace RSMadnessEngine.Api.BackgroundJobs
{
    public class TournamentSyncBackgroundJob : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TournamentSyncBackgroundJob> _logger;
        private readonly TimeSpan _syncInterval = TimeSpan.FromHours(1);

        public TournamentSyncBackgroundJob(IServiceScopeFactory serviceScopeFactory, ILogger<TournamentSyncBackgroundJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Job that runs every hour to pull ESPN data and then update each persons submitted bracket scores.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tournament Sync Background Job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
                    await syncService.SyncGameDataAndRecalculateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during tournament sync.");
                }

                await Task.Delay(_syncInterval, stoppingToken);
            }
        }
    }
}