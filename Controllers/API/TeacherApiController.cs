using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using SchoolApp.Common;
using SchoolApp.DTOs;
using SchoolApp.ViewModels.Teachers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class TeacherApiController : ControllerBase
{
    private readonly ITeacherService _service;
    private readonly IMapper _mapper;

    public TeacherApiController(ITeacherService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var teachers = await _service.GetAllAsync();
        var dtos = _mapper.Map<List<TeacherDto>>(teachers);
        var paged = new PagedResult<TeacherDto>
        {
            Items = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            TotalCount = dtos.Count(),
            PageNumber = page,
            PageSize = pageSize
        };
        return Ok(ApiResponse<PagedResult<TeacherDto>>.Ok(paged));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null)
            return NotFound(ApiResponse<object>.Fail("Teacher not found."));

        return Ok(ApiResponse<TeacherDto>.Ok(_mapper.Map<TeacherDto>(teacher)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(TeacherCreateVM model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        var teacher = _mapper.Map<Teacher>(model);
        var result = await _service.AddAsync(teacher);

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        var dto = _mapper.Map<TeacherDto>(result.Data);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<TeacherDto>.Ok(dto, result.Message));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, TeacherEditVM model)
    {
        if (!ModelState.IsValid)   // This check is crucial to prevent processing invalid data and to provide clear feedback to the client.
            return BadRequest(ApiResponse<object>.Fail("Invalid data."));

        if (id != model.Id)
            return BadRequest(ApiResponse<object>.Fail("Route id does not match model id."));

        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.Fail("Teacher not found."));

        _mapper.Map(model, existing);  // This will update the existing entity with the new values from the model.
        var result = await _service.UpdateAsync(existing); // This will save the changes to the database and return a result indicating success or failure.

        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message));

        return Ok(ApiResponse<TeacherDto>.Ok(_mapper.Map<TeacherDto>(result.Data), result.Message));
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
