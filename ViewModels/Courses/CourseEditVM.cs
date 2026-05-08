namespace SchoolApp.ViewModels.Courses
{
    public class CourseEditVM
    {
        public int Id { get; set; }
        //Needed to identify which course is being updated

        public string Name { get; set; } = string.Empty;
        // Holds edited course name

        public List<int> TeacherIds { get; set; } = new();
        // Holds selected teacher ids in edit form
    }
}
  
