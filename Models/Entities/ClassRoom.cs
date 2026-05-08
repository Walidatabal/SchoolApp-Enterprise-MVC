public class ClassRoom
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string Location { get; set; } = string.Empty;

    // Relationship with Courses (optional)
    public List<Course> Courses { get; set; } = new();
}