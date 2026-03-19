using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs;
using RSMadnessEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace RSMadnessEngine.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TeamsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all teams
        /// </summary>
        /// <returns>List<GetTeamsResponse> objects</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetTeamsResponse>>> GetAll()
        {
            var teams = await _dbContext.Teams
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

            return Ok(teams);
        }
    }
}
