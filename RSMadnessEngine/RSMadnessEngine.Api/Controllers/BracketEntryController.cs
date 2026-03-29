using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;
using System.Linq;
using RSMadnessEngine.Api.Services;
using RSMadnessEngine.Api.Services.BracketEntries.Repositories;
using RSMadnessEngine.Api.Services.BracketEntries;

namespace RSMadnessEngine.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BracketEntryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private string GetUserId() => User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
        private readonly IBracketEntryService _bracketEntryService;
        private readonly IScoringService _scoringService;

        public BracketEntryController(AppDbContext dbContext, IBracketEntryService bracketEntryService, IScoringService scoringService)
        {
            _dbContext = dbContext;
            _bracketEntryService = bracketEntryService;
            _scoringService = scoringService;
        }        

        /// <summary>
        /// Gets the bracket entry information for the logged in user
        /// </summary>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpGet("me")]
        public async Task<ActionResult<GetBracketEntryResponse>> GetMyBracketEntry()
        {
            var response = await _bracketEntryService.GetMyBracketEntryAsync(GetUserId());
            return Ok(response);
        }

        /// <summary>
        /// Creates or updates a bracket entry for the logged in user if valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpPut("me/ranks")]
        public async Task<ActionResult<GetBracketEntryResponse>> SaveRanks(SaveRanksRequest request)
        {
            var result = await _bracketEntryService.SaveRanksAsync(GetUserId(), request);
            return Ok(result);
        }

        /// <summary>
        /// Locks the bracket entry for the logged in user.
        /// </summary>
        /// <returns>GetBracketEntryResponse object</returns>
        //[HttpPost("me/submit")]
        //public async Task<ActionResult<GetBracketEntryResponse>> Submit()
        //{
        //    var userId = GetUserId();

        //    var bracketEntry = await _dbContext.BracketEntries
        //        .Include(be => be.EntryTeamRanks)
        //        .FirstOrDefaultAsync(be => be.UserId == userId);

        //    if (bracketEntry == null)
        //    {
        //        return NotFound(new { errors = new[] { "No bracket entry found. Save your rankings first." } });
        //    }

        //    if (bracketEntry.SubmittedAt != null)
        //    {
        //        return BadRequest(new { errors = new[] { "Bracket Entry already locked in." } });
        //    }

        //    var ranks = bracketEntry.EntryTeamRanks
        //        .OrderBy(r => r.Rank)
        //        .Select(r => new RankAssignment
        //        {
        //            TeamId = r.TeamId,
        //            Rank = r.Rank
        //        }).ToList();

        //    var errors = ValidateRanks(ranks);
        //    if (errors.Any())
        //    {
        //        return BadRequest(new { Errors = errors });
        //    }

        //    bracketEntry.SubmittedAt = DateTime.UtcNow;
        //    await _dbContext.SaveChangesAsync();

        //    // calculate score for the current bracket
        //    await _scoringService.CalculatecoreAsync(bracketEntry);

        //    // return fresh response
        //    var response = await _bracketEntryService.GetMyBracketEntryAsync(userId);

        //    return Ok(response);
        //}
    }
}
