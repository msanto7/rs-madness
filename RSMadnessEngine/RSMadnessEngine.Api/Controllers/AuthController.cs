using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.DTOs.Auth;
using RSMadnessEngine.Api.Services;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(DTOs.Auth.RegisterRequest request)
        {
            // validae if the email exists already
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already in use.");
            }

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName
            };

            // identity - hashes password and validates
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new AuthResponse
            {
                Token = token,
                DisplayName = user.DisplayName,
                Email = user.Email
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(DTOs.Auth.LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // validate the provided password
            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new AuthResponse
            {
                Token = token,
                DisplayName = user.DisplayName!,
                Email = user.Email!
            });
        }
    }
}
