namespace SchoolApp.ViewModels.Admin
{
    public class UserRoleVM
    {
        public string UserId { get; set; } = string.Empty;
        // Identity user id

        public string Email { get; set; } = string.Empty;
        // User email

        public bool IsAdmin { get; set; }
        // True if user has Admin role

        public bool IsUser { get; set; }
        // True if user has User role

        // ✅ الحل هنا
        public bool IsActive { get; set; }
    }
}