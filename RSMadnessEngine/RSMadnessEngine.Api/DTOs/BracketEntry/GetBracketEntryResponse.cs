namespace RSMadnessEngine.Api.DTOs.BracketEntry
{
    /// <summary>
    /// For the get bracket endpoint -- returns bracket id, submittedAt (to indicate if user has locked their selections), and list of ranked teams
    /// </summary>
    public class GetBracketEntryResponse
    {
        public int Id { get; set; }
        
        public DateTime? SubmittedAt { get; set; }

        public List<TeamRankDTO> Ranks { get; set; } = new List<TeamRankDTO>();
    }

    /// <summary>
    /// List of team info and the users rank for each team
    /// </summary>
    public class TeamRankDTO
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public int Seed { get; set; }

        public string Region { get; set; } = string.Empty;

        public int Rank { get; set; }
    }
}
