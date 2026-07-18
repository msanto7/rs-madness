namespace RSMadnessEngine.Api.DTOs.BracketEntry
{
    /// <summary>
    /// Reports the configured bracket submission deadline and whether it has already passed.
    /// </summary>
    public class SubmissionDeadlineResponse
    {
        public DateTime? DeadlineUtc { get; set; }

        public bool IsPassed { get; set; }
    }
}
