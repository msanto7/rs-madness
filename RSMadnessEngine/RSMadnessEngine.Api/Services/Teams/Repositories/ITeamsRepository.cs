using RSMadnessEngine.Api.DTOs;

namespace RSMadnessEngine.Api.Services.Teams.Repositories;

public interface ITeamsRepository
{
    Task<List<GetTeamsResponse>> GetAllAsync();
}
