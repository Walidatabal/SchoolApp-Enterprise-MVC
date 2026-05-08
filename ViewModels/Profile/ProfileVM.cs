namespace SchoolApp.ViewModels.Profile
{
    public class ProfileVM
    {
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Specialization { get; set; }

        public string ProfileType { get; set; } = string.Empty;
    }
}