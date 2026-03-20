using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.Leaderboard;
using RSMadnessEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public LeaderboardController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the leaderboard list for all users with a submitted bracket entry.
        /// </summary>
        /// <returns>List<LeaderboardEntryResponse> object.</returns>
        [HttpGet]
        public async Task<ActionResult<List<LeaderboardEntryResponse>>> GetLeaderboard()
        {
            var bracketEntries = await _dbContext.BracketEntries
                .Where(be => be.SubmittedAt != null)
                .Select(be => new
                {
                    be.User.DisplayName,
                    CurrentPoints = be.Score != null ? be.Score.CurrentPoints : 0,
                    PotentialPoints = be.Score != null ? be.Score.PotentialPoints : 0,
                    be.SubmittedAt
                })
                .OrderByDescending(be => be.CurrentPoints)
                .ThenBy(be => be.PotentialPoints)
                .ToListAsync();

            var result = bracketEntries.Select((be, index) => new LeaderboardEntryResponse
            {
                Position = index + 1,
                UserDisplayName = be.DisplayName ?? string.Empty,
                CurrentPoints = be.CurrentPoints,
                PotentialPoints = be.PotentialPoints,
                SubmittedAt = be.SubmittedAt
            }).ToList();

            return Ok(result);
        }
    }
}
