namespace RSMadnessEngine.Api.DTOs.BracketEntry
{
    public class SaveRanksRequest
    {
        public List<RankAssignment> Ranks { get; set; } = new List<RankAssignment>();
    }

    public class RankAssignment
    {
        public int TeamId { get; set; }
        public int Rank { get; set; }
    }
}
