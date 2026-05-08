namespace SchoolApp.ViewModels.Teachers
{
    public class PendingTeacherVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}