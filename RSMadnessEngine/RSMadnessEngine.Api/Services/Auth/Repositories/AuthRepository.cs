using Microsoft.AspNetCore.Identity;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<AppUser> _userManager;

    public AuthRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<AppUser?> FindByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<AppUser?> FindByIdAsync(string userId)
    {
        return _userManager.FindByIdAsync(userId);
    }

    public Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }
}
