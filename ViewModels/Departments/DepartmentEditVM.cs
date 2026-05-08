namespace SchoolApp.ViewModels.Departments
{
    public class DepartmentEditVM
    {
        public int Id { get; set; }
        // Department id used during update

        public string Name { get; set; } = string.Empty;
        // Department name entered from Edit form
    }
}