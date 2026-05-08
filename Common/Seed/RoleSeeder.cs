using Microsoft.AspNetCore.Identity;
using SchoolApp.Common.Constants;

namespace SchoolApp.Common.Seed
{
    // 🎯 Responsible ONLY for creating roles
    // ✔ Single Responsibility Principle
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            // All roles in the system
            var roles = new[]
            {
                Roles.Admin,
                Roles.Teacher,
                Roles.PendingTeacher,
                Roles.Student,
                Roles.Parent,
                Roles.User
            };

            foreach (var role in roles)
            {
                // Check if role already exists
                if (!await roleManager.RoleExistsAsync(role))
                {
                    // Create role
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}