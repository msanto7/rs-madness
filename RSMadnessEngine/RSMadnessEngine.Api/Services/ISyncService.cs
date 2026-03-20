namespace RSMadnessEngine.Api.Services
{
    public interface ISyncService
    {
        Task SyncGameDataAndRecalculateAsync();
    }
}
