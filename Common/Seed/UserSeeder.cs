using Microsoft.AspNetCore.Identity;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Identity;

namespace SchoolApp.Common.Seed
{
    // 🎯 Responsible ONLY for creating users
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            var email = DefaultUsers.AdminEmail;
            var password = DefaultUsers.GetAdminPassword(config);

            // Check if admin exists
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Assign Admin role
                    await userManager.AddToRoleAsync(user, Roles.Admin);
                }
            }
        }
    }
}