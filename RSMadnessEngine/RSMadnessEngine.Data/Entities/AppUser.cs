using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
    }
}
