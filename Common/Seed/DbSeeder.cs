using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using SchoolApp.Models.Identity;
using SchoolApp.Repositories.Interfaces;

namespace SchoolApp.Common.Seed
{
    // Main application seeder
    // Responsible for calling all seeders in correct order
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            // =========================
            // GET REQUIRED SERVICES
            // =========================

            // ASP.NET Identity Role Manager
            // Used to create roles like:
            // Admin / Teacher / Student
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ASP.NET Identity User Manager
            // Used to create application users
            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Access appsettings.json values
            // Example:
            // Seed:AdminPassword
            var config =
                serviceProvider.GetRequiredService<IConfiguration>();

            // UnitOfWork access
            // Used to seed database entities
            // مثل:
            // Students / Parents / Courses
            var unitOfWork =
                serviceProvider.GetRequiredService<IUnitOfWork>();


            // =========================
            // START DATABASE SEEDING
            // =========================

            // 1. Seed system roles first
            // لأن المستخدمين يعتمدوا على الـ Roles
            await RoleSeeder.SeedAsync(roleManager);

            // 2. Seed application users
            // Example:
            // admin@school.com
            await UserSeeder.SeedAsync(userManager, config);

            // 3. Seed parents
            // الطلاب يعتمدوا على أولياء الأمور
            //await ParentSeeder.SeedAsync(unitOfWork);

            //// 4. Seed students
            //// يستخدم ParentId من ParentSeeder
            //await StudentSeeder.SeedAsync(unitOfWork);

            // Later we can add:
            // await TeacherSeeder.SeedAsync(unitOfWork);
            // await CourseSeeder.SeedAsync(unitOfWork);
            // await EnrollmentSeeder.SeedAsync(unitOfWork);
        }
    }
}