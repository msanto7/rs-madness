using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Api.Services.BracketEntries;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BracketEntryController : ControllerBase
    {
        private readonly IBracketEntryService _bracketEntryService;

        public BracketEntryController(IBracketEntryService bracketEntryService)
        {
            _bracketEntryService = bracketEntryService;
        }

        private string GetUserId() => User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        /// <summary>
        /// Gets the bracket entry information for the logged in user.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<GetBracketEntryResponse>> GetMyBracketEntry()
        {
            var response = await _bracketEntryService.GetMyBracketEntryAsync(GetUserId());
            return Ok(response);
        }

        /// <summary>
        /// Creates or updates a bracket entry for the logged in user if valid.
        /// </summary>
        [HttpPut("me/ranks")]
        public async Task<ActionResult<GetBracketEntryResponse>> SaveRanks(SaveRanksRequest request)
        {
            var response = await _bracketEntryService.SaveRanksAsync(GetUserId(), request);
            return Ok(response);
        }

        /// <summary>
        /// Locks the bracket entry for the logged in user.
        /// </summary>
        [HttpPost("me/submit")]
        public async Task<ActionResult<GetBracketEntryResponse>> Submit()
        {
            var response = await _bracketEntryService.SubmitAsync(GetUserId());
            return Ok(response);
        }
    }
}
