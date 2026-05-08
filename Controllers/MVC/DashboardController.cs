


namespace SchoolApp.Controllers.MVC
{

    [Authorize]
    public class DashboardController : Controller
    {

        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IParentService _parentService;

        public DashboardController(
    IStudentService studentService,
    ITeacherService teacherService,
    ICourseService courseService,
    IParentService parentService)
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _courseService = courseService;
            _parentService = parentService;
        }
        public IActionResult Index()
        {
            if (User.IsInRole(Roles.Admin))
                return RedirectToAction(nameof(Admin));

            if (User.IsInRole(Roles.Teacher))
                return RedirectToAction(nameof(Teacher));

            if (User.IsInRole(Roles.Student))
                return RedirectToAction(nameof(Student));

            if (User.IsInRole(Roles.Parent))
                return RedirectToAction(nameof(Parent));

            if (User.IsInRole(Roles.PendingTeacher))
                return RedirectToAction(nameof(PendingTeacher));

            return RedirectToAction(nameof(UserDashboard));
        }
        

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Admin()
        {
            var model = new DashboardVM
            {
                StudentsCount = await _studentService.GetCountAsync(),
                TeachersCount = await _teacherService.GetCountAsync(),
                CoursesCount = await _courseService.GetCountAsync(),
                ParentsCount = await _parentService.GetCountAsync(),
                PendingTeachersCount = await _teacherService.GetPendingTeachersCountAsync()
            };

            return View(model);
        }
        
        
        
        [Authorize(Roles = Roles.Teacher)]
        public IActionResult Teacher()
        {
            return View();
        }

        [Authorize(Roles = Roles.Student)]
        public IActionResult Student()
        {
            return View();
        }

        [Authorize(Roles = Roles.Parent)]
        public IActionResult Parent()
        {
            return View();
        }

        [Authorize(Roles = Roles.PendingTeacher)]
        public IActionResult PendingTeacher()
        {
            return View();
        }

        [Authorize]
        public IActionResult UserDashboard()
        {
            return View();
        }
    }
}