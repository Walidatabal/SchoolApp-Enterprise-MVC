using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Common;
using SchoolApp.DTOs;
using SchoolApp.Models.Entities;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Departments;

namespace SchoolApp.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DepartmentApiController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IMapper _mapper;

        public DepartmentApiController(IDepartmentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var departments = await _service.GetAllAsync();
            var dtos = _mapper.Map<List<DepartmentDto>>(departments);
            var paged = new PagedResult<DepartmentDto>
            {
                Items = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = dtos.Count(),
                PageNumber = page,
                PageSize = pageSize
            };
            return Ok(ApiResponse<PagedResult<DepartmentDto>>.Ok(paged));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null)
                return NotFound(ApiResponse<object>.Fail("Department not found."));

            return Ok(ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DepartmentCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid data."));

            var department = _mapper.Map<Department>(model);
            var result = await _service.AddAsync(department);

            if (!result.Success)
                return BadRequest(ApiResponse<object>.Fail(result.Message));

            var dto = _mapper.Map<DepartmentDto>(result.Data);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<DepartmentDto>.Ok(dto, result.Message));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, DepartmentEditVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid data."));

            if (id != model.Id)
                return BadRequest(ApiResponse<object>.Fail("Route id does not match model id."));

            var department = await _service.GetByIdAsync(id);
            if (department == null)
                return NotFound(ApiResponse<object>.Fail("Department not found."));

            _mapper.Map(model, department);
            var result = await _service.UpdateAsync(department);

            if (!result.Success)
                return BadRequest(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(result.Data), result.Message));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<object>.Ok(null, result.Message));
        }
    }
}
