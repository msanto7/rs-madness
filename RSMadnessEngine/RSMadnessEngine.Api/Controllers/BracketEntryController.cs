using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RSMadnessEngine.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BracketEntryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private string GetUserId() => User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        public BracketEntryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }        

        /// <summary>
        /// Gets the bracket entry information for the logged in user
        /// </summary>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpGet("me")]
        public async Task<ActionResult<GetBracketEntryResponse>> GetMyBracketEntry()
        {
            var userId = GetUserId();

            // pull the first bracket entry for a user
            var entry = await _dbContext.BracketEntries
                .Where(be => be.UserId == userId)
                .Select(be => new GetBracketEntryResponse
                {
                    Id = be.Id,
                    SubmittedAt = be.SubmittedAt,
                    Ranks = be.EntryTeamRanks.Select(r => new TeamRankDTO
                    {
                        TeamId = r.TeamId,
                        TeamName = r.Team.Name,
                        Seed = r.Team.Seed,
                        Region = r.Team.Region,
                        Rank = r.Rank
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (entry == null)
            {
                return NotFound("No bracket entry found for the current user.");
            }
                
            return Ok(entry);
        }
    }
}
