using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Users
{
    public class ChangeUserRoleVM
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }
}