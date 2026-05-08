namespace SchoolApp.ViewModels.Students
{
    public class StudentListItemVM
    {
        public int Id { get; set; }
        // Used in Edit/Delete links

        public string Name { get; set; } = string.Empty;
        // Displayed in the students table

        public DateTime BirthDate { get; set; }
        // Displayed in the students table


        public String? ParentName { get; set; }
        // Displayed in the students table

    }
}