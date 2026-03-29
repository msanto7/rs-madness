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

        public BracketEntryController(AppDbContext dbContext, IBracketEntryService bracketEntryService)
        {
            _dbContext = dbContext;
            _bracketEntryService = bracketEntryService;
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
            var response = await _bracketEntryService.SaveRanksAsync(GetUserId(), request);
            return Ok(response);
        }

        /// <summary>
        /// Locks the bracket entry for the logged in user.
        /// </summary>
        /// <returns>GetBracketEntryResponse object</returns>
        [HttpPost("me/submit")]
        public async Task<ActionResult<GetBracketEntryResponse>> Submit()
        {
            var response = await _bracketEntryService.SubmitAsync(GetUserId());
            return Ok(response);
        }
    }
}
