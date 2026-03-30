using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.Leaderboard;
using RSMadnessEngine.Api.Services.Leaderboard;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Gets the leaderboard list for all users with a submitted bracket entry.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<LeaderboardEntryResponse>>> GetLeaderboard()
        {
            var result = await _leaderboardService.GetLeaderboardAsync();
            return Ok(result);
        }
    }
}
