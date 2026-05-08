namespace SchoolApp.ViewModels.Users
{
    public class UserListItemVM
    {
        // Identity User Id
        public string Id { get; set; } = string.Empty;

        // User email displayed in table
        public string Email { get; set; } = string.Empty;

        // User role(s)
        public string Roles { get; set; } = string.Empty;

        // Is the user locked
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }
}