        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Identity;
        using Microsoft.AspNetCore.Mvc;
        using SchoolApp.Common.Constants;
        using SchoolApp.Common.Extensions;
        using SchoolApp.Models.Common;
        using SchoolApp.Services.Interfaces;
        using SchoolApp.ViewModels.Profile;
        using System.Security.Claims;

        namespace SchoolApp.Controllers.MVC
        {
            // Any logged-in user can view their own profile
            [Authorize]
            public class ProfileController : Controller
            {
                private readonly UserManager<ApplicationUser> _userManager;
                private readonly IStudentService _studentService;
                private readonly ITeacherService _teacherService;
                private readonly ILogger<ProfileController> _logger;

                public ProfileController(
                    UserManager<ApplicationUser> userManager,
                    IStudentService studentService,
                    ITeacherService teacherService,
                    ILogger<ProfileController> logger)
                {
                    _userManager = userManager;
                    _studentService = studentService;
                    _teacherService = teacherService;
                    _logger = logger;
                }

                public async Task<IActionResult> Index()
                {
                    // Get current logged-in user id from claims
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrWhiteSpace(userId))
                        return Unauthorized();

                    // Get Identity user
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user == null)
                        return NotFound();

                    // Get user roles
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "No Role";

                    // Base profile data
                    var model = new ProfileVM
                    {
                        Email = user.Email ?? string.Empty,
                        Role = role
                    };

                    // =========================
                    // Student Profile
                    // =========================
                    if (roles.Contains(Roles.Student))
                    {
                        var student = _studentService.GetByUserIdAsync(userId);

                        model.FullName = (await student.ConfigureAwait(false))?.Name ?? "No student profile found";
                        model.BirthDate = (await student.ConfigureAwait(false))?.BirthDate;
                    }

                    // =========================
                    // Teacher Profile
                    // Role = Teacher
                    // Status = Pending / Approved / Rejected
                    // =========================
                    else if (roles.Contains(Roles.Teacher))
                    {
                        var teacher = await _teacherService.GetByUserIdAsync(userId);

                        if (teacher == null)
                        {
                            model.ProfileType = "Teacher";
                            model.FullName = "No teacher profile found";
                        }
                        else
                        {
                            model.ProfileType = teacher.Status.GetTeacherStatusText();  // e.g. "Approved Teacher", "Pending Teacher", "Rejected Teacher"

                            model.FullName = teacher.Name;
                            model.Specialization = teacher.Specialization;
                        }
                    }

                    // =========================
                    // Parent Profile
                    // Note: Add IParentService later if you want to load Parent details
                    // =========================
                    else if (roles.Contains(Roles.Parent))
                    {
                        model.ProfileType = "Parent";
                        model.FullName = user.Email ?? "Parent";
                    }

                    // =========================
                    // Normal User Profile
                    // =========================
                    else
                    {
                        model.ProfileType = "User";
                        model.FullName = user.Email ?? "User";
                    }

                    _logger.LogInformation(
                        "Profile opened by {User}. Role={Role}, ProfileType={ProfileType}",
                        user.Email,
                        model.Role,
                        model.ProfileType);

                    return View(model);
                }

        // GET: /Profile/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction(nameof(Index));
        }


    }
        }