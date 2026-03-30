using Microsoft.AspNetCore.Identity;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.Auth.Repositories;

public interface IAuthRepository
{
    Task<AppUser?> FindByEmailAsync(string email);

    Task<AppUser?> FindByIdAsync(string userId);

    Task<IdentityResult> CreateAsync(AppUser user, string password);

    Task<bool> CheckPasswordAsync(AppUser user, string password);
}
