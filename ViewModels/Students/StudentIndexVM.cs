using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.Students
{
    public class StudentIndexVM
    {
        public List<StudentListItemVM> Students { get; set; } = new();

        public string? SearchTerm { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; } = new List<SelectListItem>();

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}