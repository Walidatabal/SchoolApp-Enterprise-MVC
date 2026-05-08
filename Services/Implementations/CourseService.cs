using SchoolApp.Common;
using SchoolApp.Models;
using SchoolApp.Models.Entities;
using SchoolApp.Repositories.Interfaces;
using SchoolApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SchoolApp.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger _logger;

        public CourseService(IUnitOfWork unitOfWork, IAppLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
            => await _unitOfWork.Courses.GetAllAsync(q => q
                .Include(c => c.TeacherCourses)
                .ThenInclude(tc => tc.Teacher));

        public async Task<Course?> GetByIdAsync(int id)
            => await _unitOfWork.Courses.GetByIdAsync(id, q => q
                .Include(c => c.TeacherCourses)
                .ThenInclude(tc => tc.Teacher));

        public async Task<int> GetCountAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return courses.Count();
        }
        public async Task<ServiceResult<Course>> AddAsync(
            Course course,
            List<int> teacherIds)
        {
            var validation = await ValidateTeacherIdsAsync(teacherIds);

            if (!validation.Success)
                return ServiceResult<Course>.Fail(validation.Message);

            try
            {
                _logger.LogInfo($"Creating course: {course.Name}");

                foreach (var teacherId in teacherIds.Distinct())
                {
                    course.TeacherCourses.Add(new TeacherCourse
                    {
                        TeacherId = teacherId
                    });
                }

                await _unitOfWork.Courses.AddAsync(course);

                await _unitOfWork.CompleteAsync();

                return ServiceResult<Course>.Ok(
                    course,
                    "Course created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while saving the course.",
                    ex);

                return ServiceResult<Course>.Fail(
                    "An error occurred while saving the course.");
            }
        }

        public async Task<ServiceResult<Course>> UpdateAsync(Course course,List<int> teacherIds)
        {
            var validation = await ValidateTeacherIdsAsync(teacherIds);

            if (!validation.Success)
                return ServiceResult<Course>.Fail(validation.Message);

            try
            {
                _logger.LogInfo(
                    $"Updating course: {course.Name} (ID: {course.Id})");

                // Load existing course with relations
                var existingCourse =
                    await _unitOfWork.Courses.GetByIdAsync(
                        course.Id,
                        q => q.Include(c => c.TeacherCourses));

                if (existingCourse == null)
                    return ServiceResult<Course>.Fail("Course not found.");

                // Update editable fields
                existingCourse.Name = course.Name;

                // Remove old teacher relations
                foreach (var relation in existingCourse.TeacherCourses.ToList())
                {
                    _unitOfWork.TeacherCourses.Delete(relation);
                }

                // Add new teacher relations
                foreach (var teacherId in teacherIds.Distinct())
                {
                    existingCourse.TeacherCourses.Add(new TeacherCourse
                    {
                        TeacherId = teacherId,
                        CourseId = existingCourse.Id
                    });
                }

                _unitOfWork.Courses.Update(existingCourse);

                await _unitOfWork.CompleteAsync();

                return ServiceResult<Course>.Ok(
                    existingCourse,
                    "Course updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while updating the course.",
                    ex);

                return ServiceResult<Course>.Fail(
                    "An error occurred while updating the course.");
            }
        }
        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            // Load with related rows to avoid FK constraint violations
            var course = await _unitOfWork.Courses.GetByIdAsync(id, q => q
                .Include(c => c.TeacherCourses)
                .Include(c => c.Enrollments));

            if (course == null)
                return ServiceResult<bool>.Fail("Course not found.");

            try
            {
                _logger.LogInfo($"Deleting course: {course.Name} (ID: {course.Id})");

                foreach (var tc in course.TeacherCourses.ToList())
                    _unitOfWork.TeacherCourses.Delete(tc);

                foreach (var enrollment in course.Enrollments.ToList())
                    _unitOfWork.Enrollments.Delete(enrollment);

                _unitOfWork.Courses.Delete(course);
                await _unitOfWork.CompleteAsync(); // all 3 deletes in one transaction
                return ServiceResult<bool>.Ok(true, "Course deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the course.", ex);
                return ServiceResult<bool>.Fail("An error occurred while deleting the course.");
            }
        }

        private async Task<ServiceResult<bool>> ValidateTeacherIdsAsync(List<int> teacherIds)
        {
            if (teacherIds == null || !teacherIds.Any())
                return ServiceResult<bool>.Fail("At least one teacher must be selected.");

            var distinctIds = teacherIds.Distinct().ToList();
            var foundCount = await _unitOfWork.Teachers
                .FindAllAsync(t => distinctIds.Contains(t.Id));

            if (foundCount.Count() != distinctIds.Count)
                return ServiceResult<bool>.Fail("One or more teacher IDs are invalid.");

            return ServiceResult<bool>.Ok(true);
        }


    }
}
