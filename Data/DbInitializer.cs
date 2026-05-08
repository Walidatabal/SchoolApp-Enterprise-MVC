using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Common.Constants;
using SchoolApp.Models;
using SchoolApp.Models.Common;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data
{
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
                "Admin",
                "Teacher",
                "PendingTeacher",
                "Student",
                "Parent",
                "User"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // =========================
            // 2) SEED ADMIN USERS
            // =========================
            await CreateUserAsync(userManager, "admin@school.com", adminPassword, "Admin");
            await CreateUserAsync(userManager, "manager@school.com", adminPassword, "Admin");

            // =========================
            // 3) SEED DEPARTMENTS
            // =========================
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "قسم الرياضيات" },
                    new Department { Name = "قسم العلوم" },
                    new Department { Name = "قسم اللغة الإنجليزية" },
                    new Department { Name = "قسم الحاسب الآلي" },
                    new Department { Name = "قسم التربية البدنية" }
                );

                await context.SaveChangesAsync();
            }

            // =========================
            // 4) SEED CLASSROOMS / OFFICES
            // =========================
            if (!context.ClassRooms.Any())
            {
                context.ClassRooms.AddRange(
                    new ClassRoom { Name = "فصل 101", Capacity = 30, Location = "المبنى A - الدور الأول" },
                    new ClassRoom { Name = "فصل 102", Capacity = 28, Location = "المبنى A - الدور الأول" },
                    new ClassRoom { Name = "معمل الحاسب", Capacity = 25, Location = "المبنى B - الدور الثاني" },
                    new ClassRoom { Name = "معمل العلوم", Capacity = 24, Location = "المبنى C - الدور الأول" },
                    new ClassRoom { Name = "مكتب الإدارة", Capacity = 6, Location = "المبنى الرئيسي" }
                );

                await context.SaveChangesAsync();
            }

            // =========================
            // 5) SEED TEACHERS + USERS
            // =========================
            var teachersData = new[]
            {
                new { Name = "أحمد محمد", Email = "ahmed.teacher@school.com", Specialization = "رياضيات", Location = "مكتب 201", Status = ApprovalStatus.Approved },
                new { Name = "سارة علي", Email = "sara.teacher@school.com", Specialization = "لغة إنجليزية", Location = "مكتب 202", Status = ApprovalStatus.Approved },
                new { Name = "محمد حسن", Email = "mohamed.teacher@school.com", Specialization = "حاسب آلي", Location = "معمل الحاسب", Status = ApprovalStatus.Approved },
                new { Name = "فاطمة خالد", Email = "fatma.teacher@school.com", Specialization = "علوم", Location = "معمل العلوم", Status = ApprovalStatus.Pending },
                new { Name = "يوسف إبراهيم", Email = "yousef.teacher@school.com", Specialization = "تربية بدنية", Location = "الصالة الرياضية", Status = ApprovalStatus.Pending }
            };

            foreach (var item in teachersData)
            {
                var role = item.Status == ApprovalStatus.Approved ? "Teacher" : "PendingTeacher";

                var user = await CreateUserAsync(userManager, item.Email, "Test123!", role);

                if (!context.Teachers.Any(t => t.UserId == user.Id))
                {
                    context.Teachers.Add(new Teacher
                    {
                        Name = item.Name,
                        Specialization = item.Specialization,
                        Location = item.Location,
                        Status = item.Status,
                        UserId = user.Id
                    });
                }
            }

            await context.SaveChangesAsync();

            // =========================
            // 6) SEED STUDENTS + USERS
            // =========================
            var studentsData = new[]
            {
                new { Name = "عمر وليد", Email = "omar.student@school.com", BirthDate = new DateTime(2012, 5, 10) },
                new { Name = "ليلى أحمد", Email = "laila.student@school.com", BirthDate = new DateTime(2011, 3, 15) },
                new { Name = "خالد سامي", Email = "khaled.student@school.com", BirthDate = new DateTime(2013, 8, 22) },
                new { Name = "ملك محمد", Email = "malak.student@school.com", BirthDate = new DateTime(2012, 11, 5) },
                new { Name = "سيف علي", Email = "saif.student@school.com", BirthDate = new DateTime(2011, 9, 1) }
            };

            foreach (var item in studentsData)
            {
                var user = await CreateUserAsync(userManager, item.Email, "Test123!", "Student");

                if (!context.Students.Any(s => s.UserId == user.Id))
                {
                    context.Students.Add(new Student
                    {
                        Name = item.Name,
                        BirthDate = item.BirthDate,
                        UserId = user.Id
                    });
                }
            }

            await context.SaveChangesAsync();

            // =========================
            // 7) SEED COURSES
            // =========================
            if (!context.Courses.Any())
            {
                var classRoom101 = context.ClassRooms.FirstOrDefault(c => c.Name == "فصل 101");
                var computerLab = context.ClassRooms.FirstOrDefault(c => c.Name == "معمل الحاسب");
                var scienceLab = context.ClassRooms.FirstOrDefault(c => c.Name == "معمل العلوم");

                context.Courses.AddRange(
                    new Course { Name = "Mathematics", ClassRoomId = classRoom101?.Id },
                    new Course { Name = "English", ClassRoomId = classRoom101?.Id },
                    new Course { Name = "Computer Science", ClassRoomId = computerLab?.Id },
                    new Course { Name = "Chemistry", ClassRoomId = scienceLab?.Id },
                    new Course { Name = "Physical Education" }
                );

                await context.SaveChangesAsync();
            }

            // =========================
            // 8) SEED TEACHER-COURSE RELATIONS
            // =========================
            if (!context.TeacherCourses.Any())
            {
                var mathTeacher = context.Teachers.FirstOrDefault(t => t.Specialization == "رياضيات");
                var englishTeacher = context.Teachers.FirstOrDefault(t => t.Specialization == "لغة إنجليزية");
                var computerTeacher = context.Teachers.FirstOrDefault(t => t.Specialization == "حاسب آلي");

                var math = context.Courses.FirstOrDefault(c => c.Name == "Mathematics");
                var english = context.Courses.FirstOrDefault(c => c.Name == "English");
                var computer = context.Courses.FirstOrDefault(c => c.Name == "Computer Science");

                if (mathTeacher != null && math != null)
                    context.TeacherCourses.Add(new TeacherCourse { TeacherId = mathTeacher.Id, CourseId = math.Id });

                if (englishTeacher != null && english != null)
                    context.TeacherCourses.Add(new TeacherCourse { TeacherId = englishTeacher.Id, CourseId = english.Id });

                if (computerTeacher != null && computer != null)
                    context.TeacherCourses.Add(new TeacherCourse { TeacherId = computerTeacher.Id, CourseId = computer.Id });

                await context.SaveChangesAsync();
            }

            // =========================
            // 9) SEED ENROLLMENTS
            // =========================
            if (!context.Enrollments.Any())
            {
                var students = context.Students.Take(5).ToList();
                var courses = context.Courses.Take(4).ToList();

                foreach (var student in students)
                {
                    foreach (var course in courses.Take(2))
                    {
                        context.Enrollments.Add(new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = course.Id,
                            EnrollmentDate = DateTime.Today
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        // =========================
        // HELPER: CREATE IDENTITY USER
        // =========================
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

                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}