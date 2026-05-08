namespace SchoolApp.ViewModels.Teachers
{
    public class TeacherListItemVM
    {
        public int Id { get; set; }
        // Used in Edit/Delete actions

        public string Name { get; set; } = string.Empty;
        // Displayed in Teachers list page

        public string Specialization { get; set; } = string.Empty;
        // Displayed in Teachers list page
    }
}