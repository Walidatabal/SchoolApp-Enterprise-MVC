namespace SchoolApp.ViewModels.Admin
{
    public class DeleteUserVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // ✅ Show roles so admin knows what they're removing
        public List<string> Roles { get; set; } = new();
    }
}

