using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Admin
{
    public class CreateUserVM
    {
        public string Email { get; set; } = string.Empty;
        // New user email

        public string Password { get; set; } = string.Empty;
        // New user password

        public string ConfirmPassword { get; set; } = string.Empty;
        // Confirm password

        public string Role { get; set; } = "User";
        // Selected role

        public List<SelectListItem> Roles { get; set; } = new();
        // Dropdown list items instead of using ViewBag
    }
}