namespace RSMadnessEngine.Api.Services
{
    /// <summary>
    /// Service to calculate each submitted bracket entries scores based on our DB table.
    /// </summary>
    public interface IScoringService
    {
        Task CalculateScoresAsync();
    }
}
