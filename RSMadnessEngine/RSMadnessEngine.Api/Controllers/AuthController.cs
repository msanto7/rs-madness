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
        private const string AccessTokenCookieName = "rs_access_token";
        private const string RefreshTokenCookieName = "rs_refresh_token";
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService authService, IConfiguration config, IWebHostEnvironment environment)
        {
            _authService = authService;
            _config = config;
            _environment = environment;
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
            SetAuthCookies(response);
            return Ok(ToAuthResponse(response));
        }

        /// <summary>
        /// Generates and returns a user's token if the username and password are valid.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            SetAuthCookies(response);
            return Ok(ToAuthResponse(response));
        }

        /// <summary>
        /// Rotates a valid refresh token and issues a fresh access token cookie.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ApiUnauthorizedException("missing-refresh-token", "Session has expired.");
            }

            var response = await _authService.RefreshAsync(refreshToken);
            SetAuthCookies(response);
            return Ok(ToAuthResponse(response));
        }

        /// <summary>
        /// Revokes the current refresh token and clears auth cookies.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            ClearAuthCookies();
            return NoContent();
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

        private void SetAuthCookies(AuthSessionResponse response)
        {
            Response.Cookies.Append(AccessTokenCookieName, response.AccessToken, BuildCookieOptions(GetAccessTokenExpiration()));
            Response.Cookies.Append(RefreshTokenCookieName, response.RefreshToken, BuildCookieOptions(AsUtcOffset(response.RefreshTokenExpiresAt)));
        }

        // DateTime -> DateTimeOffset has an implicit conversion, but it trusts DateTime.Kind: Unspecified is
        // silently treated as local server time. RefreshTokenExpiresAt is always a UTC instant, so pin the
        // Kind explicitly rather than relying on callers upstream never producing an Unspecified DateTime.
        private static DateTimeOffset AsUtcOffset(DateTime value)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete(AccessTokenCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
            Response.Cookies.Delete(RefreshTokenCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
        }

        private CookieOptions BuildCookieOptions(DateTimeOffset expires)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = bool.Parse(_config["AuthCookies:Secure"] ?? (!_environment.IsDevelopment()).ToString()),
                SameSite = Enum.Parse<SameSiteMode>(_config["AuthCookies:SameSite"] ?? "Lax", ignoreCase: true),
                Expires = expires,
                Path = "/"
            };
        }

        private DateTimeOffset GetAccessTokenExpiration()
        {
            var minutes = double.Parse(_config["Jwt:AccessExpireMinutes"] ?? _config["Jwt:ExpireMinutes"] ?? "15");
            return DateTimeOffset.UtcNow.AddMinutes(minutes);
        }

        private static AuthResponse ToAuthResponse(AuthSessionResponse response)
        {
            return new AuthResponse
            {
                DisplayName = response.DisplayName,
                Email = response.Email
            };
        }
    }
}
