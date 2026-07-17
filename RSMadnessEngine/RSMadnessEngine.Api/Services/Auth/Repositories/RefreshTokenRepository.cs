using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _db.RefreshTokens.AddAsync(refreshToken);
    }

    public Task<RefreshToken?> FindByTokenHashAsync(string tokenHash)
    {
        return _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
