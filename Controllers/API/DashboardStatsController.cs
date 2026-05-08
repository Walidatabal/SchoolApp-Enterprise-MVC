
// ✅ Explicitly tell these API endpoints to use JWT scheme
// WHY: Now that the default scheme is Cookie (for MVC),
//      API controllers must explicitly opt into JWT.
//      Without this, they'd also use cookie auth — which doesn't
//      make sense for API clients (mobile apps, Postman, etc.)
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class DashboardStatsController : ControllerBase
    {
        private readonly AppDbContext _context;
        // Used to access application database tables (Students)

        private readonly UserManager<ApplicationUser> _userManager;
        // Used to access Identity users and roles (Admin, User)

        // Constructor with Dependency Injection
        // ASP.NET automatically provides these services
        public DashboardStatsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // GET: api/dashboardstats
        // =========================
        // This method returns dashboard statistics as JSON
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // =========================
            // 1) Get Students Count
            // =========================
            int studentsCount = _context.Students.Count();
            // Counts total number of students from database

            // =========================
            // 2) Get All Users
            // =========================
            var users = _userManager.Users.ToList();
            // Retrieves all registered users from Identity system

            int adminsCount = 0;
            int normalUsersCount = 0;

            // =========================
            // 3) Count Users by Roles
            // =========================
            foreach (var user in users)
            {
                // Check if user has Admin role
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    adminsCount++;
                }

                // Check if user has User role
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    normalUsersCount++;
                }
            }

            // =========================
            // 4) Prepare Result Object
            // =========================
            var result = new
            {
                StudentsCount = studentsCount,
                UsersCount = users.Count,
                AdminsCount = adminsCount,
                NormalUsersCount = normalUsersCount
            };
            // Anonymous object that will be converted to JSON automatically

            // =========================
            // 5) Return JSON Response
            // =========================
            return Ok(result);
            // Returns HTTP 200 with JSON body
        }
    
}