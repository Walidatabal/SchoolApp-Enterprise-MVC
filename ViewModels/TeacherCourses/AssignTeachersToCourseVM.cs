using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolApp.ViewModels.TeacherCourses
{
    public class AssignTeachersToCourseVM
    {
        // Selected course
        public int CourseId { get; set; }

        // Selected teachers
        public List<int> TeacherIds { get; set; } = new();

        // Dropdown data for courses
        public List<SelectListItem> Courses { get; set; } = new();

        // Multi-select data for teachers
        public List<SelectListItem> Teachers { get; set; } = new();
    }
}