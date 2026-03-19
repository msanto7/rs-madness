using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;
using System.Linq;

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
                    Ranks = be.EntryTeamRanks
                        .OrderBy(r => r.Rank)
                        .Select(r => new TeamRankDTO
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

        /// <summary>
        /// Creates or updates a bracket entry for the logged in user if valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpPut("me/ranks")]
        public async Task<ActionResult<GetBracketEntryResponse>> SaveRanks(SaveRanksRequest request)
        {
            var userId = GetUserId();

            // make sure bracket ranking is valid
            var errors = ValidateRanks(request.Ranks);
            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            var bracketEntry = await _dbContext.BracketEntries
                .Include(be => be.EntryTeamRanks)
                .FirstOrDefaultAsync(be => be.UserId == userId);

            // make sure we haven't locked in the bracket yet
            if (bracketEntry != null && bracketEntry.SubmittedAt != null)
            {
                return BadRequest("Bracket entry has already been submitted and cannot be modified.");
            }

            // add a new entry if the user has not made one yet
            if (bracketEntry == null)
            {
                bracketEntry = new BracketEntry
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.BracketEntries.Add(bracketEntry);
            }

            // overwrite the existing bracket state
            bracketEntry.EntryTeamRanks.Clear();
            foreach (var rank in request.Ranks)
            {
                bracketEntry.EntryTeamRanks.Add(new BracketEntryTeamRank
                {
                    TeamId = rank.TeamId,
                    Rank = rank.Rank
                });
            }
            await _dbContext.SaveChangesAsync();

            // return a full new copy of the bracket entry state
            var response = await _dbContext.BracketEntries
                .Where(be => be.UserId == userId)
                .Select(be => new GetBracketEntryResponse
                {
                    Id = be.Id,
                    SubmittedAt = be.SubmittedAt,
                    Ranks = be.EntryTeamRanks
                        .OrderBy(r => r.Rank)
                        .Select(r => new TeamRankDTO
                        {
                            TeamId = r.TeamId,
                            TeamName = r.Team.Name,
                            Seed = r.Team.Seed,
                            Region = r.Team.Region,
                            Rank = r.Rank
                        }).ToList()
                }).FirstOrDefaultAsync();

            return Ok(response);
        }

        /// <summary>
        /// Locks the bracket entry for the logged in user.
        /// </summary>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpPost("me/submit")]
        public async Task<ActionResult<GetBracketEntryResponse>> Submit()
        {
            var userId = GetUserId();

            var bracketEntry = await _dbContext.BracketEntries
                .Include(be => be.EntryTeamRanks)
                .FirstOrDefaultAsync(be => be.UserId == userId);

            if (bracketEntry == null)
            {
                return NotFound(new { errors = new[] { "No bracket entry found. Save your rankings first." } });
            }

            if (bracketEntry.SubmittedAt != null)
            {
                return BadRequest(new { errors = new[] { "Bracket Entry already locked in." } });
            }

            var ranks = bracketEntry.EntryTeamRanks
                .OrderBy(r => r.Rank)
                .Select(r => new RankAssignment
                {
                    TeamId = r.TeamId,
                    Rank = r.Rank
                }).ToList();

            var errors = ValidateRanks(ranks);
            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            bracketEntry.SubmittedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // return fresh response
            var response = await _dbContext.BracketEntries
                .Where(be => be.UserId == userId)
                .Select(be => new GetBracketEntryResponse
                {
                    Id = be.Id,
                    SubmittedAt = be.SubmittedAt,
                    Ranks = be.EntryTeamRanks
                        .OrderBy(r => r.Rank)
                        .Select(r => new TeamRankDTO
                        {
                            TeamId = r.TeamId,
                            TeamName = r.Team.Name,
                            Seed = r.Team.Seed,
                            Region = r.Team.Region,
                            Rank = r.Rank
                        }).ToList()
                }).FirstOrDefaultAsync();

            return Ok(response);
        }

        /// <summary>
        /// Method to validate the bracket ranking rules before saving
        /// </summary>
        /// <param name="ranks"></param>
        /// <returns></returns>
        private List<string> ValidateRanks(List<RankAssignment> ranks)
        {
            var errors = new List<string>();

            // validate rank count
            if (ranks.Count != 64)
            {
                errors.Add("Exactly 64 ranks must be assigned.");
                return errors;
            }

            // validate rank values
            var rankValues = ranks.Select(r => r.Rank).ToList();

            if (rankValues.Any(r => r < 1 || r > 64))
            {
                errors.Add("Ranks must be between 1 and 64.");
            }

            if (rankValues.Distinct().Count() != 64)
            {
                errors.Add("Ranks must be unique.");
            }

            if (rankValues.Sum() != 2080)
            {
                errors.Add("Ranks must sum to 2080");
            }

            // validate teams
            var teamIds = ranks.Select(r => r.TeamId).ToList();
            if (teamIds.Distinct().Count() != 64)
            {
                errors.Add("Duplicate team Ids found.");
            }

            return errors;
        }
    }
}
