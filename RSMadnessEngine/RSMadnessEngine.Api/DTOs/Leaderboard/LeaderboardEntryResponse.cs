namespace RSMadnessEngine.Api.DTOs.Leaderboard
{
    public class LeaderboardEntryResponse
    {
        public int Position { get; set; }
        
        public string UserDisplayName { get; set; } = string.Empty;

        public int CurrentPoints { get; set; }

        public int PotentialPoints { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }
}
