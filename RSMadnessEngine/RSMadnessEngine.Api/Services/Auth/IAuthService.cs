using RSMadnessEngine.Api.DTOs.Auth;

namespace RSMadnessEngine.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<CurrentUserResponse> GetCurrentUserAsync(string userId);
}
