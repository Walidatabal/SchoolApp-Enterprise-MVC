using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Students
{
    public class StudentCreateVM
    {
        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }
            = new List<SelectListItem>();
    }
}