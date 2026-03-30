using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs;
using RSMadnessEngine.Api.Services.Teams;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        /// <summary>
        /// Gets all teams.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<GetTeamsResponse>>> GetAll()
        {
            var teams = await _teamsService.GetAllAsync();
            return Ok(teams);
        }
    }
}
