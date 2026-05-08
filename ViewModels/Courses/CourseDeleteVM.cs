namespace SchoolApp.ViewModels.Courses
{
    public class CourseDeleteVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Show which teachers are assigned — so admin knows the full impact
        public List<string> TeacherNames { get; set; } = new();

        // ✅ Show how many enrollments will be deleted along with the course
        // WHY: Deleting a course cascades — all student enrollments for that
        //      course are also deleted from the Enrollments table.
        //      The user MUST be warned about this before confirming.
        public int EnrollmentCount { get; set; }
    }
}   