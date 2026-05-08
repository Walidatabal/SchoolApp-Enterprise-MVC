using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Students;

namespace SchoolApp.Controllers.MVC
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IStudentService _service;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(
            IStudentService service,
            ILogger<StudentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var students = await _service.GetAllWithParentsAsync();

            var model = students.Select(student => new StudentListItemVM
            {
                Id = student.Id,
                Name = student.Name,
                BirthDate = student.BirthDate,
                ParentName = student.Parent != null
                    ? student.Parent.FullName
                    : null
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.BuildCreateVMAsync();
            return View(vm);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await _service.RebuildCreateVMAsync(vm);
                return View(vm);
            }

            var student = new Student
            {
                Name = vm.Name,
                BirthDate = vm.BirthDate,
                ParentId = vm.ParentId
            };

            var result = await _service.AddAsync(student);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                vm = await _service.RebuildCreateVMAsync(vm);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.BuildEditVMAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await _service.RebuildEditVMAsync(vm);
                return View(vm);
            }

            var student = new Student
            {
                Id = vm.Id,
                Name = vm.Name,
                BirthDate = vm.BirthDate,
                ParentId = vm.ParentId
            };

            var result = await _service.UpdateAsync(student);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                vm = await _service.RebuildEditVMAsync(vm);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Teacher)]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.BuildDetailsVMAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.BuildDetailsVMAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _service.DeleteAsync(id);

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