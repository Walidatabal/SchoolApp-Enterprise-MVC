namespace SchoolApp.DTOs
{
    public class EnrollmentDto
    {
        public int StudentId { get; set; }
        // Student ID

        public string StudentName { get; set; } = string.Empty;
        // Student name فقط (بدل object كامل)

        public int CourseId { get; set; }
        // Course ID

        public string CourseName { get; set; } = string.Empty;
        // Course name فقط
    }
}