namespace SchoolApp.ViewModels.Parents
{
    public class ParentListItemVM
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        // Number of linked students
        public int StudentsCount { get; set; }
    }
}