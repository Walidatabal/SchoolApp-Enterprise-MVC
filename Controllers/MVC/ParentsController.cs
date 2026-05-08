using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Common;
using SchoolApp.Common.Constants;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Parents;

namespace SchoolApp.Controllers.MVC
{
    [Authorize(Roles = Roles.Admin)]
    public class ParentsController : Controller
    {
        private readonly IParentService _parentService;

        public ParentsController(IParentService parentService)
        {
            _parentService = parentService;
        }

        // GET: /Parents
        public async Task<IActionResult> Index()
        {
            var parents = await _parentService.GetAllAsync();

            var vm = parents.Select(p => new ParentListItemVM
            {
                Id = p.Id,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                StudentsCount = p.Students?.Count ?? 0
            }).ToList();

            return View(vm);
        }

        // GET: /Parents/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ParentCreateVM());
        }

        // POST: /Parents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParentCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var parent = new Parent
            {
                FullName = vm.FullName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address
            };

            var result = await _parentService.AddAsync(parent);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Parents/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var parent = await _parentService.GetByIdAsync(id);

            if (parent == null)
                return NotFound();

            var vm = new ParentEditVM
            {
                Id = parent.Id,
                FullName = parent.FullName,
                PhoneNumber = parent.PhoneNumber,
                Email = parent.Email,
                Address = parent.Address
            };

            return View(vm);
        }

        // POST: /Parents/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ParentEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var parent = new Parent
            {
                Id = vm.Id,
                FullName = vm.FullName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address
            };

            var result = await _parentService.UpdateAsync(parent);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Parents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _parentService.DeleteAsync(id);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}