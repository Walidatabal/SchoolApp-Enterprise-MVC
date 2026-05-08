using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Departments;

namespace SchoolApp.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllAsync();

            var model = departments.Select(d => new DepartmentListItemVM
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _departmentService.AddAsync(new Department
            {
                Name = model.Name
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);

            if (dept == null)
                return NotFound();

            var model = new DepartmentEditVM
            {
                Id = dept.Id,
                Name = dept.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentEditVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dept = await _departmentService.GetByIdAsync(model.Id);

            if (dept == null)
                return NotFound();

            dept.Name = model.Name;

            var result = await _departmentService.UpdateAsync(dept);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);

            if (dept == null)
                return NotFound();

            var model = new DepartmentDeleteVM
            {
                Id = dept.Id,
                Name = dept.Name
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DepartmentDeleteVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _departmentService.DeleteAsync(model.Id);

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