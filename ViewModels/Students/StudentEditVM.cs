using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Students
{
    public class StudentEditVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }
            = new List<SelectListItem>();
    }
}