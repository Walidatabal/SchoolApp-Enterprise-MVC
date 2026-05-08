using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Account
{
    public class RegisterVM
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        // Student / Teacher / Parent / User
        public string SelectedRole { get; set; } = string.Empty;

        // Used as Student.Name or Teacher.Name
        public string FullName { get; set; } = string.Empty;

        // Only used when selected role is Teacher
        public string? Specialization { get; set; }

        // Used in Register dropdown
        public List<SelectListItem> Roles { get; set; } = new();
    }
}