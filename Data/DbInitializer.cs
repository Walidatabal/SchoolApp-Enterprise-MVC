using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Common.Constants;
using SchoolApp.Models;
using SchoolApp.Models.Common;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data
{
    // =========================================================
    // DATABASE INITIALIZER
    // =========================================================
    // Purpose:
    // 1. Create required roles
    // 2. Create admin users
    // 3. Clear old test data
    // 4. Insert fresh professional test data
    //
    // NOTE:
    // This is for DEVELOPMENT / TESTING only.
    // Do NOT use data deletion like this in production.
    // =========================================================
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var config = services.GetRequiredService<IConfiguration>();

            var adminPassword = DefaultUsers.GetAdminPassword(config);

            // =========================
            // 1) SEED ROLES
            // =========================
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
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // =========================
            // 2) SEED ADMIN USERS
            // =========================
            await CreateUserAsync(userManager, "admin@school.com", adminPassword, Roles.Admin);
            await CreateUserAsync(userManager, "manager@school.com", adminPassword, Roles.Admin);

            // =========================
            // 3) CLEAR OLD TEST DATA
            // =========================
            // Delete child tables first because of relationships.
            context.Enrollments.RemoveRange(context.Enrollments);
            context.TeacherCourses.RemoveRange(context.TeacherCourses);
            context.Courses.RemoveRange(context.Courses);
            context.Students.RemoveRange(context.Students);
            context.Teachers.RemoveRange(context.Teachers);
            context.Parents.RemoveRange(context.Parents);
            context.ClassRooms.RemoveRange(context.ClassRooms);
            context.Departments.RemoveRange(context.Departments);

            await context.SaveChangesAsync();

            // =========================
            // 4) SEED DEPARTMENTS
            // =========================
            var departments = new List<Department>
            {
                new Department { Name = "Mathematics Department" },
                new Department { Name = "Science Department" },
                new Department { Name = "English Department" },
                new Department { Name = "Computer Science Department" },
                new Department { Name = "Physical Education Department" }
            };

            context.Departments.AddRange(departments);
            await context.SaveChangesAsync();

            // =========================
            // 5) SEED CLASSROOMS
            // =========================
            var classrooms = new List<ClassRoom>
            {
                new ClassRoom { Name = "Class 101", Capacity = 30, Location = "Building A - First Floor" },
                new ClassRoom { Name = "Class 102", Capacity = 28, Location = "Building A - First Floor" },
                new ClassRoom { Name = "Computer Lab", Capacity = 25, Location = "Building B - Second Floor" },
                new ClassRoom { Name = "Science Lab", Capacity = 24, Location = "Building C - First Floor" },
                new ClassRoom { Name = "Sports Hall", Capacity = 40, Location = "Main Building" }
            };

            context.ClassRooms.AddRange(classrooms);
            await context.SaveChangesAsync();

            // =========================
            // 6) SEED PARENTS
            // =========================
            var parents = new List<Parent>
            {
                new Parent { FullName = "Ahmed Hassan", PhoneNumber = "99990001", Address = "Kuwait City" },
                new Parent { FullName = "Mohamed Ali", PhoneNumber = "99990002", Address = "Hawally" },
                new Parent { FullName = "Sara Khaled", PhoneNumber = "99990003", Address = "Salmiya" },
                new Parent { FullName = "Omar Mahmoud", PhoneNumber = "99990004", Address = "Farwaniya" },
                new Parent { FullName = "Fatma Adel", PhoneNumber = "99990005", Address = "Jahra" },
                new Parent { FullName = "Youssef Ibrahim", PhoneNumber = "99990006", Address = "Mangaf" },
                new Parent { FullName = "Mona Samir", PhoneNumber = "99990007", Address = "Fahaheel" },
                new Parent { FullName = "Khaled Mostafa", PhoneNumber = "99990008", Address = "Mahboula" }
            };

            context.Parents.AddRange(parents);
            await context.SaveChangesAsync();

            // =========================
            // 7) SEED TEACHERS + USERS
            // =========================
            var teachersData = new[]
            {
                new { Name = "Ahmed Mohamed", Email = "ahmed.teacher@school.com", Specialization = "Mathematics", Location = "Office 201", Status = ApprovalStatus.Approved },
                new { Name = "Sara Ali", Email = "sara.teacher@school.com", Specialization = "English", Location = "Office 202", Status = ApprovalStatus.Approved },
                new { Name = "Mohamed Hassan", Email = "mohamed.teacher@school.com", Specialization = "Computer Science", Location = "Computer Lab", Status = ApprovalStatus.Approved },
                new { Name = "Fatma Khaled", Email = "fatma.teacher@school.com", Specialization = "Science", Location = "Science Lab", Status = ApprovalStatus.Pending },
                new { Name = "Youssef Ibrahim", Email = "youssef.teacher@school.com", Specialization = "Physical Education", Location = "Sports Hall", Status = ApprovalStatus.Pending },
                new { Name = "Noura Adel", Email = "noura.teacher@school.com", Specialization = "Chemistry", Location = "Science Lab", Status = ApprovalStatus.Approved }
            };

            foreach (var item in teachersData)
            {
                var role = item.Status == ApprovalStatus.Approved
                    ? Roles.Teacher
                    : Roles.PendingTeacher;

                var user = await CreateUserAsync(userManager, item.Email, "Test123!", role);

                context.Teachers.Add(new Teacher
                {
                    Name = item.Name,
                    Specialization = item.Specialization,
                    Location = item.Location,
                    Status = item.Status,
                    UserId = user.Id
                });
            }

            await context.SaveChangesAsync();

            // =========================
            // 8) SEED STUDENTS + USERS
            // =========================
            var parentIds = await context.Parents
                .Select(p => p.Id)
                .ToListAsync();

            for (int i = 1; i <= 50; i++)
            {
                var email = $"student{i}@school.com";

                var user = await CreateUserAsync(
                    userManager,
                    email,
                    "Test123!",
                    Roles.Student);

                context.Students.Add(new Student
                {
                    Name = $"Student {i}",
                    BirthDate = new DateTime(
                        2010 + (i % 4),
                        (i % 12) + 1,
                        (i % 28) + 1),

                    UserId = user.Id,

                    // Assign parent in rotation
                    ParentId = parentIds[(i - 1) % parentIds.Count]
                });
            }

            await context.SaveChangesAsync();

            // =========================
            // 9) SEED COURSES
            // =========================
            var class101 = await context.ClassRooms.FirstAsync(c => c.Name == "Class 101");
            var class102 = await context.ClassRooms.FirstAsync(c => c.Name == "Class 102");
            var computerLab = await context.ClassRooms.FirstAsync(c => c.Name == "Computer Lab");
            var scienceLab = await context.ClassRooms.FirstAsync(c => c.Name == "Science Lab");
            var sportsHall = await context.ClassRooms.FirstAsync(c => c.Name == "Sports Hall");

            var courses = new List<Course>
            {
                new Course { Name = "Mathematics", ClassRoomId = class101.Id },
                new Course { Name = "English", ClassRoomId = class102.Id },
                new Course { Name = "Computer Science", ClassRoomId = computerLab.Id },
                new Course { Name = "Chemistry", ClassRoomId = scienceLab.Id },
                new Course { Name = "Physics", ClassRoomId = scienceLab.Id },
                new Course { Name = "Physical Education", ClassRoomId = sportsHall.Id }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // =========================
            // 10) SEED TEACHER-COURSE RELATIONS
            // =========================
            var teachers = await context.Teachers.ToListAsync();
            var allCourses = await context.Courses.ToListAsync();

            foreach (var course in allCourses)
            {
                var teacher = teachers.FirstOrDefault(t =>
                    course.Name.Contains(t.Specialization) ||
                    t.Specialization.Contains(course.Name));

                if (teacher != null)
                {
                    context.TeacherCourses.Add(new TeacherCourse
                    {
                        TeacherId = teacher.Id,
                        CourseId = course.Id
                    });
                }
            }

            await context.SaveChangesAsync();

            // =========================
            // 11) SEED ENROLLMENTS
            // =========================
            var students = await context.Students.ToListAsync();
            var courseList = await context.Courses.ToListAsync();

            foreach (var student in students)
            {
                // Each student enrolls in 2 courses
                var firstCourse = courseList[student.Id % courseList.Count];
                var secondCourse = courseList[(student.Id + 1) % courseList.Count];

                context.Enrollments.Add(new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = firstCourse.Id,
                    EnrollmentDate = DateTime.Today
                });

                context.Enrollments.Add(new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = secondCourse.Id,
                    EnrollmentDate = DateTime.Today
                });
            }

            await context.SaveChangesAsync();
        }

        // =========================================================
        // HELPER METHOD: CREATE IDENTITY USER
        // =========================================================
        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ",
                        createResult.Errors.Select(e => e.Description));

                    throw new Exception($"Failed to create user {email}: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ",
                        roleResult.Errors.Select(e => e.Description));

                    throw new Exception($"Failed to add role {role} to {email}: {errors}");
                }
            }

            return user;
        }
    }
}