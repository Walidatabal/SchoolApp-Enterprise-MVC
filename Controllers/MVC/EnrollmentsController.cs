using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Common.Constants;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Enrollments;

namespace SchoolApp.Controllers.MVC
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
    public class EnrollmentsController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(IEnrollmentService enrollmentService, IStudentService studentService, ICourseService courseService, ILogger<EnrollmentsController> logger)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _enrollmentService.GetAllAsync();
            var model = enrollments.Select(e => new EnrollmentListItemVM
            {
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                StudentName = e.Student?.Name ?? "N/A",
                CourseName = e.Course?.Name ?? "N/A",
                EnrollmentDate = e.EnrollmentDate
            }).ToList();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollmentCreateVM model)
        {
            if (!ModelState.IsValid) { await LoadDropdownsAsync(); return View(model); }

            var result = await _enrollmentService.AddRangeAsync(model);
            if (!result.Success) { ModelState.AddModelError("", result.Message); await LoadDropdownsAsync(); return View(model); }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int studentId, int courseId)
        {
            var result = await _enrollmentService.DeleteAsync(studentId, courseId);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync()
        {
            var students = await _studentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();

            ViewBag.Students = students.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
            ViewBag.Courses = courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        }
    }
}
