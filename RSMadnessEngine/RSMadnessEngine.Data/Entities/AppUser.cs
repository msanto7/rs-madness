using Microsoft.AspNetCore.Identity;

namespace RSMadnessEngine.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
    }
}
