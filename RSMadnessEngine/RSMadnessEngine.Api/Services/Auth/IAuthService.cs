using RSMadnessEngine.Api.DTOs.Auth;

namespace RSMadnessEngine.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthSessionResponse> RegisterAsync(RegisterRequest request);

    Task<AuthSessionResponse> LoginAsync(LoginRequest request);

    Task<AuthSessionResponse> RefreshAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);

    Task<CurrentUserResponse> GetCurrentUserAsync(string userId);
}
