namespace SchoolApp.ViewModels.Enrollments
{
    public class EnrollmentCreateVM
    {
        public int StudentId { get; set; }
        // Selected student

        public List<int> CourseIds { get; set; } = new();
        // Selected courses
    }
}