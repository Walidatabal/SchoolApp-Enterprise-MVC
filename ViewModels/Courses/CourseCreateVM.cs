namespace SchoolApp.ViewModels.Courses
{
    public class CourseCreateVM
    {
        public string Name { get; set; } = string.Empty;
        // Holds course name entered in create form

        public List<int> TeacherIds { get; set; } = new();
        // Holds selected teacher ids for many-to-many relation
    }
}