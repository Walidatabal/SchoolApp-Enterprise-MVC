namespace SchoolApp.ViewModels.Departments
{
    public class DepartmentDeleteVM
    {
        public int Id { get; set; }
        // Used to identify department

        public string Name { get; set; } = string.Empty;
        // Display only in confirm page
    }
}