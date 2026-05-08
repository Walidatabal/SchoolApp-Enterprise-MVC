namespace SchoolApp.ViewModels.TeacherCourses
{
    public class TeacherCourseListItemVM
    {
        // Teacher display name
        public string TeacherName { get; set; } = string.Empty;

        // Course display name
        public string CourseName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public int CourseId { get; set; }


    }
}