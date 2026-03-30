using RSMadnessEngine.Api.DTOs.Auth;
using RSMadnessEngine.Api.Errors;
using RSMadnessEngine.Api.Services.Auth.Repositories;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // validate if the email exists already
        var existingUser = await _authRepository.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ApiConflictException("email-already-in-use", "Email already in use.");
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        // identity - hashes password and validates
        var result = await _authRepository.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ApiValidationException(
                "registration-failed",
                "Registration failed.",
                result.Errors.Select(e => e.Description));
        }

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new ApiUnauthorizedException("invalid-credentials", "Invalid email or password.");
        }

        // validate the provided password
        var valid = await _authRepository.CheckPasswordAsync(user, request.Password);
        if (!valid)
        {
            throw new ApiUnauthorizedException("invalid-credentials", "Invalid email or password.");
        }

        return BuildAuthResponse(user);
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(string userId)
    {
        var user = await _authRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApiUnauthorizedException("invalid-user", "User is not authorized.");
        }

        return new CurrentUserResponse
        {
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };
    }

    private AuthResponse BuildAuthResponse(AppUser user)
    {
        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };
    }
}
