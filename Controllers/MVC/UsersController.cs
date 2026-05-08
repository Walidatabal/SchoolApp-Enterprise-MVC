using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Common;
using SchoolApp.Models.Identity;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Teachers;
using SchoolApp.ViewModels.Users;

namespace SchoolApp.Controllers.MVC
{
    // Only Admin can manage users and teacher approval
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeacherService _teacherService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger,
            ITeacherService teacherService,
            RoleManager<IdentityRole> roleManager   )
        {
            _userManager = userManager;
            _logger = logger;
            _teacherService = teacherService;
            _roleManager = roleManager;
        }

        // Display all Identity users with their roles
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserListItemVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserListItemVM
                {
                    Id = user.Id,
                    Email = user.Email ?? "N/A",
                    Roles = roles.Any()
                        ? string.Join(", ", roles)
                        : "No Role",

                    IsLocked = user.LockoutEnd != null &&
                               user.LockoutEnd > DateTimeOffset.UtcNow,
                    IsActive = user.IsActive

                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Lock user for 100 years
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "User locked successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Remove lock
            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "User unlocked successfully.";

            return RedirectToAction(nameof(Index));
        }
        // Show teachers waiting for admin approval
 

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var vm = new ResetUserPasswordVM
            {
                UserId = user.Id,
                Email = user.Email ?? "N/A"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetUserPasswordVM model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            TempData["Success"] = "Password reset successfully.";

            return RedirectToAction(nameof(Index));
        }
        // Approve teacher using TeacherId
        // =========================================================
        // APPROVE TEACHER
        // Admin approves pending teacher accounts
        //
        // WHY IMPORTANT?
        // Teacher registration uses approval workflow.
        // New teachers cannot fully access the system
        // until Admin approves them.
        // =========================================================

        //[Authorize(Roles = Roles.Admin)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ApproveTeacher(int teacherId)
        //{
        //    // =====================================================
        //    // Get teacher from database
        //    // =====================================================
        //    var teacher = await _teacherService.GetByIdAsync(teacherId);

        //    // Teacher not found
        //    if (teacher == null)
        //    {
        //        TempData["Error"] = "Teacher not found.";

        //        return RedirectToAction(nameof(PendingTeachers));
        //    }

        //    // =====================================================
        //    // Update approval status
        //    // =====================================================
        //    teacher.Status = ApprovalStatus.Approved;

        //    // =====================================================
        //    // Save changes using service layer
        //    // =====================================================
        //    var result = await _teacherService.UpdateAsync(teacher);

        //    // =====================================================
        //    // If update failed
        //    // =====================================================
        //    if (!result.Success)
        //    {
        //        TempData["Error"] = result.Message;

        //        return RedirectToAction(nameof(PendingTeachers));
        //    }

        //    // =====================================================
        //    // Success message
        //    // =====================================================
        //    TempData["Success"] =
        //        $"Teacher '{teacher.Name}' approved successfully.";

        //    // =====================================================
        //    // Redirect back to pending teachers page
        //    // =====================================================
        //    return RedirectToAction(nameof(PendingTeachers));
        //}

        // Reject teacher using TeacherId

        // It displays a form to change a user's role. It retrieves the user by ID, gets their current roles, and populates a view model with the user's information and available roles for selection.
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ChangeUserRoleVM
            {
                UserId = user.Id,
                Email = user.Email ?? "N/A",
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    })
                    .ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeUserRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                // Reload roles if validation fails
                model.Roles = _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    })
                    .ToList();

                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                TempData["Error"] = "Failed to remove current role.";
                return RedirectToAction(nameof(Index));
            }

            // Add selected role
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

                if (!addResult.Succeeded)
                {
                    TempData["Error"] = "Failed to assign new role.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Success"] = "User role updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserListItemVM
            {
                Id = user.Id,
                Email = user.Email ?? "N/A",
                Roles = roles.Any() ? string.Join(", ", roles) : "No Role",
                IsLocked = user.LockoutEnd != null &&
                           user.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to deactivate user.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "User deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to activate user.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "User activated successfully.";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FixTeacherAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains(Roles.Teacher))
            {
                TempData["Error"] = "This user is not a Teacher.";
                return RedirectToAction(nameof(Index));
            }

            var teacher = await _teacherService.GetByUserIdAsync(user.Id);

            if (teacher == null)
            {
                var result = await _teacherService.AddAsync(new Teacher
                {
                    Name = user.Email ?? "Teacher",
                    Email = user.Email ?? "",
                    UserId = user.Id,
                    Specialization = "Not assigned",
                    Status = ApprovalStatus.Approved
                });

                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                teacher.Status = ApprovalStatus.Approved;

                var result = await _teacherService.UpdateAsync(teacher);

                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            user.IsActive = true;
            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Teacher account fixed and activated.";

            return RedirectToAction(nameof(Index));
        }
    }
}