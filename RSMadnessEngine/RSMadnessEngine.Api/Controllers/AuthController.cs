using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.Auth;
using RSMadnessEngine.Api.Errors;
using RSMadnessEngine.Api.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ApiUnauthorizedException("missing-user-claim", "User is not authorized.");
            }

            return userId;
        }

        /// <summary>
        /// Allows a user to register and returns a valid token if successful.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Generates and returns a user's token if the username and password are valid.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the logged-in user's information.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserResponse>> Me()
        {
            var response = await _authService.GetCurrentUserAsync(GetUserId());
            return Ok(response);
        }
    }
}
