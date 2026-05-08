using Microsoft.AspNetCore.Identity;

namespace SchoolApp.Models.Identity
{
    public class ApplicationUser :  IdentityUser
    {
        // Business status:
        // true  = user can use the system
        // false = user exists but is inactive
        public bool IsActive { get; set; } = true;
    }
}