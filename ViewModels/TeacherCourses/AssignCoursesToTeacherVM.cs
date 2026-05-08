using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.TeacherCourses
{
    public class AssignCoursesToTeacherVM
    {
        // Selected teacher id from dropdown
        public int TeacherId { get; set; }

        // Selected course ids from multi-select
        public List<int> CourseIds { get; set; } = new();

        // Dropdown list for teachers
        public List<SelectListItem> Teachers { get; set; } = new();

        // Multi-select list for courses
        public List<SelectListItem> Courses { get; set; } = new();
    }
}