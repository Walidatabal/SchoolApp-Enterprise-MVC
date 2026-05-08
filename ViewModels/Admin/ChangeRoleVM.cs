
namespace SchoolApp.ViewModels.Admin
{
    public class ChangeRoleVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // ✅ Describes the action: "promote to Admin" or "remove Admin role from"
        // Reused by both MakeAdmin and RemoveAdmin confirmation pages
        public string Action { get; set; } = string.Empty;
    }
}