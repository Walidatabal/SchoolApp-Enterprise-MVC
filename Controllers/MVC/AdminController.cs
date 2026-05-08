using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.ViewModels.Account;
using SchoolApp.ViewModels.Admin;

namespace SchoolApp.Controllers.MVC
{
    // ✅ [Authorize(Roles = "Admin")] at class level — already correct
    // This means EVERY action in this controller requires the Admin role.
    // No need to repeat it on individual actions — the class-level attribute
    // covers all of them automatically.
    // Compare to other controllers where [Authorize] is at class level
    // and [Authorize(Roles = "Admin")] is repeated per action — here we
    // don't need that because ALL actions are Admin-only by definition.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ Users list — reads TempData to show success/error messages
        // after any action (delete, promote, demote) redirects back here
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRoleVM>();

            foreach (var user in users)
            {
                model.Add(new UserRoleVM
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
                    IsUser = await _userManager.IsInRoleAsync(user, "User"),
                    IsActive = user.IsActive
                });
            }

            return View(model);
        }

        // =========================
        // CREATE USER
        // =========================

        [HttpGet]
        public IActionResult CreateUser()
        {
            var model = new CreateUserVM
            {
                Roles = GetRoles()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetRoles();
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "This email is already registered.");
                model.Roles = GetRoles();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = "User created successfully.";
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.Roles = GetRoles();
            return View(model);
        }        // =========================
        // DELETE USER — GET (confirmation page)
        // =========================

        // ✅ ADDED: GET confirmation page before deleting a user
        // BEFORE: DeleteUser was POST-only — the Users.cshtml view had a
        //         form with a submit button that deleted immediately.
        //         There was no "are you sure?" step.
        //
        // AFTER:  Clicking "Delete" goes to this GET action first,
        //         which shows a confirmation page with the user's email.
        //         Only after clicking "Yes, Delete" does the POST fire.
        //
        // WHY THIS MATTERS FOR ADMIN:
        //   Accidentally deleting a user account is serious — it removes
        //   their login, role, and access permanently. A confirmation page
        //   forces a deliberate second click, preventing fat-finger mistakes.
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            // ✅ Prevent admin from navigating to their own delete confirmation
            // If they somehow reach this page for their own account, redirect immediately.
            if (user.Email == User.Identity!.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Users");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new DeleteUserVM
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,

                // ✅ Show the user's current roles on the confirmation page
                // WHY: Admin must know if they're deleting another Admin —
                //      that's a much more impactful action than deleting a regular User.
                //      Showing roles makes the consequence clear before confirming.
                Roles = roles.ToList()
            };

            return View(model);
        }

        // ✅ POST: Actual delete — only fires after confirmation page
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            // ✅ Double-check self-delete on POST too
            // WHY: Never rely on only the GET check. Someone could craft
            //      a direct POST request to bypass the GET confirmation page.
            //      Always re-validate the business rule on the POST itself.
            if (user.Email == User.Identity!.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Users");
            }

            var result = await _userManager.DeleteAsync(user);

            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? $"User '{user.Email}' deleted successfully."
                : "Failed to delete user.";

            return RedirectToAction("Users");
        }

        // =========================
        // MAKE ADMIN — GET (confirmation page)
        // =========================

        // ✅ ADDED: confirmation page before promoting a user to Admin
        // BEFORE: MakeAdmin was POST-only with a button on the Users list.
        //         One accidental click promoted any user to Admin instantly.
        //
        // AFTER: GET shows who you're about to promote, POST does it.
        //
        // WHY: Giving someone Admin access is a critical security decision.
        //      It grants full access to create, edit, and delete everything.
        //      This must never happen by accident.
        [HttpGet]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var model = new ChangeRoleVM
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Action = "promote to Admin"
                // Action text is shown on the confirmation page:
                // "Are you sure you want to promote to Admin [email]?"
            };

            return View("ConfirmRoleChange", model);
            // ✅ Reuse a single shared confirmation view for both
            //    MakeAdmin and RemoveAdmin — same layout, different action text
        }

        [HttpPost, ActionName("MakeAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdminConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            TempData["Success"] = $"'{user.Email}' is now an Admin.";
            return RedirectToAction("Users");
        }

        // =========================
        // REMOVE ADMIN — GET (confirmation page)
        // =========================

        [HttpGet]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // ✅ Prevent removing your own Admin role
            // WHY: If the only Admin removes their own Admin role,
            //      nobody can access the Admin panel anymore — the app is locked.
            if (user.Email == User.Identity!.Name)
            {
                TempData["Error"] = "You cannot remove your own Admin role.";
                return RedirectToAction("Users");
            }

            var model = new ChangeRoleVM
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Action = "remove Admin role from"
            };

            return View("ConfirmRoleChange", model);
        }

        [HttpPost, ActionName("RemoveAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdminConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // ✅ Double-check self-demotion on POST too
            if (user.Email == User.Identity!.Name)
            {
                TempData["Error"] = "You cannot remove your own Admin role.";
                return RedirectToAction("Users");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");

            TempData["Success"] = $"Admin role removed from '{user.Email}'.";
            return RedirectToAction("Users");
        }

        // =========================
        // RESET PASSWORD
        // =========================

        // ✅ Already correct pattern (GET form + POST submit) — no changes needed
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var model = new ResetPasswordVM
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Password reset successfully for '{user.Email}'.";
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        private List<SelectListItem> GetRoles()
        {
            return _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name!,
                    Text = r.Name!
                })
                .ToList();
        }
    }
}