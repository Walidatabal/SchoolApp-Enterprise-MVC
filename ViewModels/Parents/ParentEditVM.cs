namespace SchoolApp.ViewModels.Parents
{
    public class ParentEditVM
    {
        public int Id { get; set; }

        // Parent full name
        public string FullName { get; set; } = string.Empty;

        // Parent phone number
        public string PhoneNumber { get; set; } = string.Empty;

        // Optional email
        public string? Email { get; set; }

        // Optional address
        public string? Address { get; set; }
    }
}