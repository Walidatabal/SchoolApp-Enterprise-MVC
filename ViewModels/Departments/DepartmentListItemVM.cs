namespace SchoolApp.ViewModels.Departments
{
    public class DepartmentListItemVM
    {
        public int Id { get; set; }
        // Department id used for Edit/Delete actions

        public string Name { get; set; } = string.Empty;
        // Department name displayed in Index page
    }
}