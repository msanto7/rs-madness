namespace RSMadnessEngine.Api.DTOs
{
    public class GetTeamsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seed { get; set; }
        public string Region { get; set; } = string.Empty;
    }
}
