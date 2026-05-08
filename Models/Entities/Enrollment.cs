namespace SchoolApp.Models.Entities
{
    public class Enrollment : Common.BaseEntity
    {
        public int StudentId { get; set; }
        // FK → Student

        public Student? Student { get; set; }
        // Navigation → Student

        public int CourseId { get; set; }
        // FK → Course

        public Course? Course { get; set; }
        // Navigation → Course

        public DateTime EnrollmentDate { get; set; }
        // Stores date when student enrolled in the course
    }
}