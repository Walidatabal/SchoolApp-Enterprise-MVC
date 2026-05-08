using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Courses;

namespace SchoolApp.Controllers.MVC
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ITeacherService teacherService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _teacherService = teacherService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllAsync();
            var model = courses.Select(c => new CourseListItemVM
            {
                Id = c.Id,
                Name = c.Name,
                TeacherName = c.TeacherCourses.Any()
                    ? string.Join(", ", c.TeacherCourses.Select(tc => tc.Teacher!.Name))
                    : "N/A"
            }).ToList();
            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create()
        {
            await LoadTeachersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(CourseCreateVM model)
        {
            if (!ModelState.IsValid) { await LoadTeachersAsync(); return View(model); }

            var result = await _courseService.AddAsync(new Course { Name = model.Name }, model.TeacherIds);
            if (!result.Success) { ModelState.AddModelError("", result.Message); await LoadTeachersAsync(); return View(model); }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();

            var model = new CourseEditVM
            {
                Id = course.Id,
                Name = course.Name,
                TeacherIds = course.TeacherCourses.Select(tc => tc.TeacherId).ToList()
            };
            await LoadTeachersAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(CourseEditVM model)
        {
            if (!ModelState.IsValid) { await LoadTeachersAsync(); return View(model); }

            var course = await _courseService.GetByIdAsync(model.Id);
            if (course == null) return NotFound();

            course.Name = model.Name;
            var result = await _courseService.UpdateAsync(course, model.TeacherIds);
            if (!result.Success) { ModelState.AddModelError("", result.Message); await LoadTeachersAsync(); return View(model); }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();

            return View(new CourseDeleteVM
            {
                Id = course.Id,
                Name = course.Name,
                TeacherNames = course.TeacherCourses.Where(tc => tc.Teacher != null).Select(tc => tc.Teacher!.Name).ToList(),
                EnrollmentCount = course.Enrollments?.Count ?? 0
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _courseService.DeleteAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTeachersAsync()
        {
            var teachers = await _teacherService.GetAllAsync();
            ViewBag.Teachers = teachers.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();
        }
    }
}
