namespace SchoolApp.ViewModels.Courses
{
    public class CourseListItemVM
    {
        public int Id { get; set; }
        // Used for Edit/Delete actions

        public string Name { get; set; } = string.Empty;
        // Displayed in courses list

        public string TeacherName { get; set; } = string.Empty;
        // Display teacher name in the courses table
    }
}