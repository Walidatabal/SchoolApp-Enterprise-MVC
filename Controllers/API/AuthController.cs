

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITeacherService _teacherService;  // ✅ add this field
    private readonly UserManager<ApplicationUser> _userManager;   // UserManager is used for user-related operations like finding users and checking passwords.
    private readonly SignInManager<ApplicationUser> _signInManager; // SignInManager is used for handling sign-in operations, including checking password sign-in and enforcing lockout policies.
    private readonly ITokenService _tokenService;  // ITokenService is a custom service for generating JWT tokens based on user information and roles.

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ITeacherService teacherService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _teacherService = teacherService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)  // The Login action method handles POST requests to using async to    "api/auth/login". It accepts a LoginDto object containing the user's email and password.
    {
        var user = await _userManager.FindByEmailAsync(model.Email);  // Find the user by email. If the user is not found, return an Unauthorized response with a generic message to prevent user enumeration attacks.

        // Return the same generic message for both "user not found" and "wrong password"
        // to avoid leaking which emails are registered (user enumeration attack).
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });

        // ✅ FIX 2a: block inactive users
        if (!user.IsActive)
            return Unauthorized(new { message = "Account is inactive." });

        // ✅ FIX 2b: block unapproved teachers
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(Roles.Teacher) || roles.Contains(Roles.PendingTeacher))
        {
            var teacher = await _teacherService.GetByUserIdAsync(user.Id);

            if (teacher == null)
                return Unauthorized(new { message = "Teacher profile not found." });

            if (teacher.Status == ApprovalStatus.Pending)
                return Unauthorized(new { message = "Account pending approval." });

            if (teacher.Status == ApprovalStatus.Rejected)
                return Unauthorized(new { message = "Account was rejected." });
        }


        // CheckPasswordSignInAsync enforces lockout — after MaxFailedAccessAttempts
        // wrong attempts the account locks for DefaultLockoutTimeSpan (15 min).
        // CheckPasswordAsync (old code) silently bypassed this entirely.
        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            model.Password,
            lockoutOnFailure: true);

        if (result.IsLockedOut)    //  If the account is locked out due to too many failed attempts, return a 429 Too Many Requests status with a message indicating the lockout duration.
            return StatusCode(StatusCodes.Status429TooManyRequests,
                new { message = "Account locked. Too many failed attempts. Try again in 15 minutes." });

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });

        
        var token = _tokenService.CreateToken(user, roles);

        return Ok(new { token });
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("You are authorized.");
    }

}