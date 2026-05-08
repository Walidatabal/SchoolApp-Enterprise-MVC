using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.Models;
using SchoolApp.ViewModels.TeacherCourses;


namespace SchoolApp.Services.Implementations
{
    public class TeacherCourseService : ITeacherCourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TeacherCourseService> _logger;

        public TeacherCourseService(
            IUnitOfWork unitOfWork,
            ILogger<TeacherCourseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<TeacherCourse>> GetAllAsync()
        {
            return await _unitOfWork.TeacherCourses.GetAllAsync(q =>
                q.Include(tc => tc.Teacher)
                 .Include(tc => tc.Course));
        }

        public async Task<List<int>> GetCourseIdsByTeacherIdAsync(int teacherId)
        {
            var teacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

            return teacherCourses
                .Where(tc => tc.TeacherId == teacherId)
                .Select(tc => tc.CourseId)
                .ToList();
        }

        public async Task<ServiceResult> AssignCoursesToTeacherAsync(
            int teacherId,
            List<int> courseIds)
        {
            _logger.LogInformation(
                "AssignCoursesToTeacher started for TeacherId {TeacherId}",
                teacherId);

            if (teacherId <= 0)
                return ServiceResult.Fail("Please select a teacher.");

            if (courseIds == null || !courseIds.Any())
                return ServiceResult.Fail("Please select at least one course.");

            var distinctCourseIds = courseIds.Distinct().ToList();

            var allTeacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

            var oldRelations = allTeacherCourses
                .Where(tc => tc.TeacherId == teacherId)
                .ToList();

            foreach (var relation in oldRelations)
            {
                _unitOfWork.TeacherCourses.Delete(relation);
            }

            foreach (var courseId in distinctCourseIds)
            {
                await _unitOfWork.TeacherCourses.AddAsync(new TeacherCourse
                {
                    TeacherId = teacherId,
                    CourseId = courseId
                });
            }

            await _unitOfWork.CompleteAsync();

            return ServiceResult.Ok("Teacher courses assigned successfully.");
        }

        //public async Task<ServiceResult> UpdateAsync(AssignCoursesToTeacherVM vm)
        //{
        //    if (vm.TeacherId <= 0)
        //        return ServiceResult.Fail("Please select a teacher.");

        //    if (vm.CourseIds == null || !vm.CourseIds.Any())
        //        return ServiceResult.Fail("Please select at least one course.");

        //    var distinctCourseIds = vm.CourseIds.Distinct().ToList();

        //    var allTeacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

        //    var oldRelations = allTeacherCourses
        //        .Where(tc => tc.TeacherId == vm.TeacherId)
        //        .ToList();

        //    foreach (var relation in oldRelations)
        //    {
        //        _unitOfWork.TeacherCourses.Delete(relation);
        //    }

        //    foreach (var courseId in distinctCourseIds)
        //    {
        //        await _unitOfWork.TeacherCourses.AddAsync(new TeacherCourse
        //        {
        //            TeacherId = vm.TeacherId,
        //            CourseId = courseId
        //        });
        //    }

        //    await _unitOfWork.CompleteAsync();

        //    return ServiceResult.Ok("Teacher courses updated successfully.");
        //}

        public async Task<ServiceResult<bool>> UpdateAsync(AssignCoursesToTeacherVM vm)
        {
            if (vm.TeacherId <= 0)
                return ServiceResult<bool>.Fail("Please select a teacher.");

            if (vm.CourseIds == null || !vm.CourseIds.Any())
                return ServiceResult<bool>.Fail("Please select at least one course.");

            var distinctCourseIds = vm.CourseIds.Distinct().ToList();

            var allTeacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

            var oldRelations = allTeacherCourses
                .Where(tc => tc.TeacherId == vm.TeacherId)
                .ToList();

            foreach (var relation in oldRelations)
            {
                _unitOfWork.TeacherCourses.Delete(relation);
            }

            foreach (var courseId in distinctCourseIds)
            {
                await _unitOfWork.TeacherCourses.AddAsync(new TeacherCourse
                {
                    TeacherId = vm.TeacherId,
                    CourseId = courseId
                });
            }

            await _unitOfWork.CompleteAsync();

            return ServiceResult<bool>.Ok(true, "Teacher courses updated successfully.");
        }

        public async Task<TeacherCourseListItemVM?> GetByIdsAsync(int teacherId, int courseId)
        {
            var teacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync(q =>
                q.Include(tc => tc.Teacher)
                 .Include(tc => tc.Course));

            var relation = teacherCourses.FirstOrDefault(tc =>
                tc.TeacherId == teacherId &&
                tc.CourseId == courseId);

            if (relation == null)
                return null;

            return new TeacherCourseListItemVM
            {
                TeacherId = relation.TeacherId,
                CourseId = relation.CourseId,
                TeacherName = relation.Teacher?.Name ?? "N/A",
                CourseName = relation.Course?.Name ?? "N/A"
            };
        }
        public async Task<ServiceResult<bool>> DeleteAsync(int teacherId, int courseId)
        {
            var allTeacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

            var relation = allTeacherCourses.FirstOrDefault(tc =>
                tc.TeacherId == teacherId &&
                tc.CourseId == courseId);

            if (relation == null)
                return ServiceResult<bool>.Fail("Teacher course assignment not found.");

            _unitOfWork.TeacherCourses.Delete(relation);

            await _unitOfWork.CompleteAsync();

            return ServiceResult<bool>.Ok(true, "Teacher course assignment deleted successfully.");
        }


        public void Test()
        {
            _logger.LogInformation("This is info log");
            _logger.LogWarning("This is warning");
            _logger.LogError("This is error");
        }

        public async  Task<AssignCoursesToTeacherVM?> GetEditVMAsync(int teacherId, int courseId)
        {
            var allTeacherCourses = await _unitOfWork.TeacherCourses.GetAllAsync();

            var relation = allTeacherCourses.FirstOrDefault(tc =>
                tc.TeacherId == teacherId &&
                tc.CourseId == courseId);

            if (relation == null)
                return null;

            var teachers = await _unitOfWork.Teachers.GetAllAsync();
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return new AssignCoursesToTeacherVM
            {
                TeacherId = relation.TeacherId,

                CourseIds = new List<int>
        {
            relation.CourseId
        },

                Teachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList(),

                Courses = courses.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
        }
    }
    
}