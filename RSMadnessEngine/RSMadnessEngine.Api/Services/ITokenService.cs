using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services;

public interface ITokenService
{
    string GenerateToken(AppUser user);
}