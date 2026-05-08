namespace SchoolApp.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        // Course Id

        public string Name { get; set; } = string.Empty;
        // Course name

        public List<string> Teachers { get; set; } = new();
        // Teacher names only
    }
}

