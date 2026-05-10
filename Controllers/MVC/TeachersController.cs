using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Implementations;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Teachers;

namespace SchoolApp.Controllers.MVC
{
    [Authorize]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _service;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(ITeacherService service, ILogger<TeachersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Teachers Index opened by {User}.", User.Identity?.Name);
            var teachers = await _service.GetAllAsync();
            var model = teachers.Select(t => new TeacherListItemVM
            {
                Id = t.Id,
                Name = t.Name,
                Specialization = t.Specialization
            }).ToList();
            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(TeacherCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var teacher = new Teacher { Name = model.Name, Specialization = model.Specialization };
            var result = await _service.AddAsync(teacher);

            if (!result.Success) { ModelState.AddModelError("", result.Message); return View(model); }

            TempData["Success"] = "Teacher created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _service.GetByIdAsync(id);
            if (teacher == null) return NotFound();
            return View(new TeacherEditVM { Id = teacher.Id, Name = teacher.Name, Specialization = teacher.Specialization });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(TeacherEditVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var teacher = new Teacher { Id = model.Id, Name = model.Name, Specialization = model.Specialization };
            var result = await _service.UpdateAsync(teacher);

            if (!result.Success) { ModelState.AddModelError("", result.Message); return View(model); }

            TempData["Success"] = "Teacher updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingTeachers()
        {
            var model = await _service.GetPendingAsync();

            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveTeacher(int teacherId)
        {
            var result = await _service.ApproveTeacherAsync(teacherId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(PendingTeachers));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectTeacher(int teacherId)
        {
            var result = await _service.RejectTeacherAsync(teacherId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(PendingTeachers));
        }
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _service.GetByIdAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _service.DeleteAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        // =========================
        // TEACHER DETAILS
        // =========================
        // WHY:
        // Allows authenticated users to view teacher information.
        // Admins and Teachers can access this page.
        // =========================
        // TEACHER DETAILS
        // =========================
        [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _service.GetByIdAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Entity -> ViewModel
            var vm = new TeacherDetailsVM
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Specialization = teacher.Specialization,
                Status = teacher.Status
            };

            return View(vm);
        }

    }
}
