namespace SchoolApp.ViewModels.Enrollments
{
    public class EnrollmentListItemVM
    {
        public int StudentId { get; set; }
        // Needed for delete action

        public int CourseId { get; set; }
        // Needed for delete action

        public string StudentName { get; set; } = string.Empty;
        // Display student name

        public string CourseName { get; set; } = string.Empty;
        // Display course name

        public DateTime EnrollmentDate { get; set; }
        // Display enrollment date
    }
}