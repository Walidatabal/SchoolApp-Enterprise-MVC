using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchoolApp.Common;
using SchoolApp.DTOs;
using SchoolApp.ViewModels.Students;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class StudentApiController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IMapper _mapper;

    public StudentApiController(IStudentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var students = await _service.GetAllAsync();
        var result = _mapper.Map<List<StudentDto>>(students);
        return Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound(ApiResponse<object>.Fail("Student not found."));

        return Ok(ApiResponse<StudentDto>.Ok(_mapper.Map<StudentDto>(student)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(StudentCreateVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        var student = _mapper.Map<Student>(model);
        var result = await _service.AddAsync(student);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        var dto = _mapper.Map<StudentDto>(result.Data);

        // 201 Created with Location header pointing to the new resource
        return CreatedAtAction(nameof(GetById), new { id = dto.Id },
            ApiResponse<StudentDto>.Ok(dto, result.Message));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, StudentEditVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        if (id != model.Id)
            return BadRequest(ApiResponse<object>.Fail("Route id does not match model id."));

        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.Fail("Student not found."));

        _mapper.Map(model, existing);
        var result = await _service.UpdateAsync(existing);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        return Ok(ApiResponse<StudentDto>.Ok(_mapper.Map<StudentDto>(result.Data), result.Message));
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
