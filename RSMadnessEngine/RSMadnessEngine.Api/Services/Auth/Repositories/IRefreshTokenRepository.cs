using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);

    Task<RefreshToken?> FindByTokenHashAsync(string tokenHash);

    Task SaveChangesAsync();
}
