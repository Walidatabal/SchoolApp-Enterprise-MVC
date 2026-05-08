namespace SchoolApp.DTOs
{
    public class TeacherDto
    {
        public int Id { get; set; }
        // Teacher id

        public string Name { get; set; } = string.Empty;
        // Teacher name

        public string Specialization { get; set; } = string.Empty;
        // Teacher specialization
    }
}