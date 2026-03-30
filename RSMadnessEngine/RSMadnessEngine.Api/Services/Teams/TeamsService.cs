using RSMadnessEngine.Api.DTOs;
using RSMadnessEngine.Api.Services.Teams.Repositories;

namespace RSMadnessEngine.Api.Services.Teams;

public class TeamsService : ITeamsService
{
    private readonly ITeamsRepository _teamsRepository;

    public TeamsService(ITeamsRepository teamsRepository)
    {
        _teamsRepository = teamsRepository;
    }

    public Task<List<GetTeamsResponse>> GetAllAsync()
    {
        return _teamsRepository.GetAllAsync();
    }
}
