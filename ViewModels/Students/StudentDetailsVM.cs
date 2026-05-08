namespace SchoolApp.ViewModels.Students
{
    public class StudentDetailsVM
    {
        // Student primary key
        public int Id { get; set; }

        // Student full name
        public string Name { get; set; } = string.Empty;

        // Date of birth
        public DateTime BirthDate { get; set; }

        // Parent display name
        // Used in Details page instead of sending full Parent entity
        public string ParentName { get; set; } = "No Parent";
    }
}