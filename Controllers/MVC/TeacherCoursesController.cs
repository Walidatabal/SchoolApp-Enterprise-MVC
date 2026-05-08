using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Common;
using SchoolApp.Common.Constants;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.TeacherCourses;

namespace SchoolApp.Controllers.MVC
{
    [Authorize(Roles = Roles.Admin)]
    public class TeacherCoursesController : Controller
    {
        private readonly ITeacherCourseService _teacherCourseService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly ILogger<TeacherCoursesController> _logger;

        public TeacherCoursesController(
            ITeacherCourseService teacherCourseService,
            ITeacherService teacherService,
            ICourseService courseService,
            ILogger<TeacherCoursesController> logger)
        {
            _teacherCourseService = teacherCourseService;
            _teacherService = teacherService;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var teacherCourses = await _teacherCourseService.GetAllAsync();
            var model = teacherCourses.Select(tc => new TeacherCourseListItemVM
            {
                TeacherId = tc.TeacherId,
                CourseId = tc.CourseId,
                TeacherName = tc.Teacher?.Name ?? "N/A",
                CourseName = tc.Course?.Name ?? "N/A"
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            var model = await BuildVM();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignCoursesToTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                model = await RebuildLists(model);
                return View(model);
            }

            var result = await _teacherCourseService.AssignCoursesToTeacherAsync(
                model.TeacherId,
                model.CourseIds);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;

            model = await RebuildLists(model);
            return View(model);
        }

        private async Task<AssignCoursesToTeacherVM> BuildVM()
        {
            var teachers = (await _teacherService.GetAllAsync()).ToList();
            var courses = (await _courseService.GetAllAsync()).ToList();

            return new AssignCoursesToTeacherVM
            {
                Teachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList(),

                Courses = courses.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
        }

        private async Task<AssignCoursesToTeacherVM> RebuildLists(AssignCoursesToTeacherVM model)
        {
            var teachers = (await _teacherService.GetAllAsync()).ToList();
            var courses = (await _courseService.GetAllAsync()).ToList();

            model.Teachers = teachers.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            model.Courses = courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int teacherId, int courseId)
        {
            var vm = await _teacherCourseService.GetEditVMAsync(teacherId, courseId);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AssignCoursesToTeacherVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _teacherCourseService.UpdateAsync(vm);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(vm);
            }

            TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int teacherId, int courseId)
        {
            var vm = await _teacherCourseService.GetByIdsAsync(teacherId, courseId);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int teacherId, int courseId)
        {
            var result = await _teacherCourseService.DeleteAsync(teacherId, courseId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}