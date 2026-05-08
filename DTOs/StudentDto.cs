namespace SchoolApp.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        // Student id

        public string Name { get; set; } = string.Empty;
        // Student name

        public DateTime BirthDate { get; set; }
        // Student birth date
    }
}