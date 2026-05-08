using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Common;
using SchoolApp.Models.Entities;
using SchoolApp.Models.Identity;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Account;

namespace SchoolApp.Controllers.MVC
{
    // Handles Register, Login, Logout, and AccessDenied
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITeacherService _teacherService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITeacherService teacherService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _teacherService = teacherService;
            _logger = logger;
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterVM
            {
                // Load roles allowed for public registration
                Roles = LoadPublicRoles()
            };

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            // If validation fails, reload dropdown roles and return view
            if (!ModelState.IsValid)
            {
                model.Roles = LoadPublicRoles();
                return View(model);
            }

            // Create Identity user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            // If Identity creation failed, show errors
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                model.Roles = LoadPublicRoles();
                return View(model);
            }

            var selectedRole = model.SelectedRole;

            // Security: never allow public registration as Admin
            if (selectedRole == Roles.Admin)
                selectedRole = Roles.User;

            // Teacher registration needs a Teacher profile + approval workflow
            if (selectedRole == Roles.Teacher)
            {
                await _userManager.AddToRoleAsync(user, Roles.PendingTeacher);  // ✅ limited role until approved

                var teacher = new Teacher
                {
                    Name = model.FullName,
                    Email = model.Email,
                    UserId = user.Id,
                    Specialization = model.Specialization ?? string.Empty,
                    Status = ApprovalStatus.Pending
                };

                var teacherResult = await _teacherService.AddAsync(teacher);

                // Rollback Identity user if Teacher profile creation fails
                if (!teacherResult.Success)
                {
                    await _userManager.DeleteAsync(user);

                    ModelState.AddModelError("", teacherResult.Message);
                    model.Roles = LoadPublicRoles();

                    return View(model);
                }

                TempData["Info"] = "Your teacher account is pending admin approval.";

                // Do not sign in pending teachers automatically
                return RedirectToAction(nameof(Login));
            }

            // Student / Parent / User registration
            await _userManager.AddToRoleAsync(user, selectedRole);

            // Sign in immediately after registration
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Dashboard decides where to go based on role
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, go to role-based dashboard
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View(new LoginVM());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: /Account/Login

        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login model submitted.");
                return View(model);
            }

            _logger.LogInformation("Login attempt for {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Your account is inactive. Please contact administrator.");
                return View(model);
            }

            // ✅ FIX 1: check teacher status BEFORE signing in.
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(Roles.Teacher) || roles.Contains(Roles.PendingTeacher))
            {
                var teacher = await _teacherService.GetByUserIdAsync(user.Id);

                if (teacher == null)
                {
                    ModelState.AddModelError("", "Teacher profile not found. Please contact administrator.");
                    return View(model);
                }

                if (teacher.Status == ApprovalStatus.Pending)
                {
                    ModelState.AddModelError("", "Your teacher account is pending approval.");
                    return View(model);
                }

                if (teacher.Status == ApprovalStatus.Rejected)
                {
                    ModelState.AddModelError("", "Your teacher account was rejected.");
                    return View(model);
                }
            }

            // All pre-checks passed — sign in now.
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is locked for 15 minutes due to multiple failed login attempts.");
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            return RedirectToAction("Index", "Dashboard");
        }
        // All pre-checks passed — sign in now.

        // POST: /Account/Logout
        // Must be POST because logout changes authentication state
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        // Roles allowed during public registration
        private List<SelectListItem> LoadPublicRoles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = Roles.Student, Text = "Student" },
                new SelectListItem { Value = Roles.Teacher, Text = "Teacher" },
                new SelectListItem { Value = Roles.Parent, Text = "Parent" },
                new SelectListItem { Value = Roles.User, Text = "Normal User" }
            };
        }
    }
}