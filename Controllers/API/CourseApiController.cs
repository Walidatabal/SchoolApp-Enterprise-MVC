using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchoolApp.Common;
using SchoolApp.DTOs;
using SchoolApp.ViewModels.Courses;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class CourseApiController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly IMapper _mapper;

    public CourseApiController(ICourseService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var courses = await _service.GetAllAsync();

        var totalCount = courses.Count();

        var result = courses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Name
            })
            .ToList();

        return Ok(new PagedResult<object>
        {
            Items = result,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound(ApiResponse<object>.Fail("Course not found."));

        var dto = new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Teachers = course.TeacherCourses.Where(tc => tc.Teacher != null)
                .Select(tc => tc.Teacher!.Name).ToList()
        };
        return Ok(ApiResponse<CourseDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CourseCreateVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        var course = new Course { Name = model.Name };
        var result = await _service.AddAsync(course, model.TeacherIds);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        var dto = new CourseDto
        {
            Id = result.Data!.Id,
            Name = result.Data.Name,
            Teachers = result.Data.TeacherCourses.Where(tc => tc.Teacher != null)
                .Select(tc => tc.Teacher!.Name).ToList()
        };
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<CourseDto>.Ok(dto, result.Message));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CourseEditVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        if (id != model.Id)
            return BadRequest(ApiResponse<object>.Fail("Route id does not match model id."));

        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound(ApiResponse<object>.Fail("Course not found."));

        course.Name = model.Name;
        var result = await _service.UpdateAsync(course, model.TeacherIds);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        var dto = new CourseDto
        {
            Id = result.Data!.Id,
            Name = result.Data.Name,
            Teachers = result.Data.TeacherCourses.Where(tc => tc.Teacher != null)
                .Select(tc => tc.Teacher!.Name).ToList()
        };
        return Ok(ApiResponse<CourseDto>.Ok(dto, result.Message));
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
