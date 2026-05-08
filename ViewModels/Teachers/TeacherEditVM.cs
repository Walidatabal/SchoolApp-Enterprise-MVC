namespace SchoolApp.ViewModels.Teachers
{
    public class TeacherEditVM
    {
        public int Id { get; set; }
        // Needed to know which teacher record should be updated

        public string Name { get; set; } = string.Empty;
        // Holds edited teacher name

        public string Specialization { get; set; } = string.Empty;
        // Holds edited teacher specialization
    }
}