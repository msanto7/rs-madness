using RSMadnessEngine.Api.DTOs.Auth;
using RSMadnessEngine.Api.Errors;
using RSMadnessEngine.Api.Services.Auth.Repositories;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthService(
        IAuthRepository authRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IConfiguration config)
    {
        _authRepository = authRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<AuthSessionResponse> RegisterAsync(RegisterRequest request)
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

        return await BuildAuthSessionResponseAsync(user);
    }

    public async Task<AuthSessionResponse> LoginAsync(LoginRequest request)
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

        return await BuildAuthSessionResponseAsync(user);
    }

    public async Task<AuthSessionResponse> RefreshAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var existingToken = await _refreshTokenRepository.FindByTokenHashAsync(tokenHash);

        if (existingToken?.User == null || !existingToken.IsActive)
        {
            throw new ApiUnauthorizedException("invalid-refresh-token", "Session has expired.");
        }

        if (IsSessionExpired(existingToken))
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.SaveChangesAsync();
            throw new ApiUnauthorizedException("invalid-refresh-token", "Session has expired.");
        }

        var now = DateTime.UtcNow;
        var response = await BuildAuthSessionResponseAsync(existingToken.User, existingToken.SessionCreatedAt, now);
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.LastUsedAt = now;
        existingToken.ReplacedByTokenHash = _tokenService.HashRefreshToken(response.RefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return response;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);
        var existingToken = await _refreshTokenRepository.FindByTokenHashAsync(tokenHash);

        if (existingToken == null || existingToken.RevokedAt != null)
        {
            return;
        }

        existingToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.SaveChangesAsync();
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

    private async Task<AuthSessionResponse> BuildAuthSessionResponseAsync(AppUser user)
    {
        var now = DateTime.UtcNow;

        return await BuildAuthSessionResponseAsync(user, now, now);
    }

    private async Task<AuthSessionResponse> BuildAuthSessionResponseAsync(AppUser user, DateTime sessionCreatedAt, DateTime lastUsedAt)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        var absoluteSessionDays = double.Parse(_config["Jwt:AbsoluteSessionExpireDays"] ?? _config["Jwt:RefreshExpireDays"] ?? "14");
        var absoluteSessionExpiresAt = sessionCreatedAt.AddDays(absoluteSessionDays);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            SessionCreatedAt = sessionCreatedAt,
            LastUsedAt = lastUsedAt,
            ExpiresAt = absoluteSessionExpiresAt
        });
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthSessionResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = absoluteSessionExpiresAt,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };
    }

    private bool IsSessionExpired(RefreshToken refreshToken)
    {
        var now = DateTime.UtcNow;
        var idleTimeoutMinutes = double.Parse(_config["Jwt:IdleSessionTimeoutMinutes"] ?? "60");
        var absoluteSessionDays = double.Parse(_config["Jwt:AbsoluteSessionExpireDays"] ?? _config["Jwt:RefreshExpireDays"] ?? "14");

        return refreshToken.LastUsedAt.AddMinutes(idleTimeoutMinutes) <= now
            || refreshToken.SessionCreatedAt.AddDays(absoluteSessionDays) <= now;
    }
}
