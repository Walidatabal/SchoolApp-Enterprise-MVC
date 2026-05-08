using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchoolApp.Common;
using SchoolApp.DTOs;
using SchoolApp.ViewModels.Enrollments;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class EnrollmentApiController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentApiController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var enrollments = await _service.GetAllAsync();
        var dtos = enrollments.Select(e => new EnrollmentDto
        {
            StudentId = e.StudentId,
            StudentName = e.Student?.Name ?? "N/A",
            CourseId = e.CourseId,
            CourseName = e.Course?.Name ?? "N/A"
        }).ToList();

        var paged = new PagedResult<EnrollmentDto>
        {
            Items = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            TotalCount = dtos.Count(),
            PageNumber = page,
            PageSize = pageSize
        };
        return Ok(ApiResponse<PagedResult<EnrollmentDto>>.Ok(paged));
    }

    [HttpGet("{studentId}/{courseId}")]
    public async Task<IActionResult> GetById(int studentId, int courseId)
    {
        var enrollment = await _service.GetByIdAsync(studentId, courseId);
        if (enrollment == null)
            return NotFound(ApiResponse<object>.Fail("Enrollment not found."));

        var dto = new EnrollmentDto
        {
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student?.Name ?? "N/A",
            CourseId = enrollment.CourseId,
            CourseName = enrollment.Course?.Name ?? "N/A"
        };
        return Ok(ApiResponse<EnrollmentDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(EnrollmentCreateVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        var result = await _service.AddRangeAsync(model);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        var dtos = result.Data!.Select(e => new EnrollmentDto
        {
            StudentId = e.StudentId,
            StudentName = "N/A",
            CourseId = e.CourseId,
            CourseName = "N/A"
        }).ToList();

        return Ok(ApiResponse<IEnumerable<EnrollmentDto>>.Ok(dtos, result.Message));
    }

    [HttpDelete("{studentId}/{courseId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int studentId, int courseId)
    {
        var result = await _service.DeleteAsync(studentId, courseId);
        if (!result.Success)
            return NotFound(ApiResponse<object>.Fail(result.Message));

        return Ok(ApiResponse<object>.Ok(null, result.Message));
    }
}
