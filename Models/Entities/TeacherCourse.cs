namespace SchoolApp.Models
{
    public class TeacherCourse
    {
        // Foreign Key -> Teacher
        public int TeacherId { get; set; }

        // Navigation Property -> Teacher
        public Teacher Teacher { get; set; }

        // Foreign Key -> Course
        public int CourseId { get; set; }

        // Navigation Property -> Course
        public Course Course { get; set; }
    }
}

