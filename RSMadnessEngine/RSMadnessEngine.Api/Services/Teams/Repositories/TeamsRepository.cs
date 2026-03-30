using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Api.DTOs;
using RSMadnessEngine.Data;

namespace RSMadnessEngine.Api.Services.Teams.Repositories;

public class TeamsRepository : ITeamsRepository
{
    private readonly AppDbContext _dbContext;

    public TeamsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<GetTeamsResponse>> GetAllAsync()
    {
        return _dbContext.Teams
            .OrderBy(t => t.Region)
            .ThenBy(t => t.Seed)
            .Select(t => new GetTeamsResponse
            {
                Id = t.Id,
                Name = t.Name,
                Seed = t.Seed,
                Region = t.Region
            })
            .ToListAsync();
    }
}
