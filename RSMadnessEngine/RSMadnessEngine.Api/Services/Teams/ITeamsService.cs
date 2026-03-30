using RSMadnessEngine.Api.DTOs;

namespace RSMadnessEngine.Api.Services.Teams;

public interface ITeamsService
{
    Task<List<GetTeamsResponse>> GetAllAsync();
}
