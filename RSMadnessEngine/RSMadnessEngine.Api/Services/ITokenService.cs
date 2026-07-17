using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
