using SchoolApp.Common;
using SchoolApp.Models.Entities;
using SchoolApp.Repositories.Interfaces;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Enrollments;

namespace SchoolApp.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger _logger;

        public EnrollmentService(IUnitOfWork unitOfWork, IAppLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Load enrollments with Student and Course navigation properties
        public async Task<IEnumerable<Enrollment>> GetAllAsync()
                    => await _unitOfWork.Enrollments.GetAllAsync(q => q
                .Include(e => e.Student)
                .Include(e => e.Course));

        public async Task<Enrollment?> GetByIdAsync(int studentId, int courseId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetAllAsync(q => q
                .Include(e => e.Student)
                .Include(e => e.Course));

            return enrollments.FirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<ServiceResult<List<Enrollment>>> AddRangeAsync(EnrollmentCreateVM model)
        {
            var validation = await ValidateBeforeSaveAsync(model);
            if (!validation.Success)
                return ServiceResult<List<Enrollment>>.Fail(validation.Message);

            try
            {
                var distinctCourseIds = model.CourseIds.Distinct().ToList();

                // Check existing enrollments with a DB query, not a full table load
                var existing = await _unitOfWork.Enrollments.GetAllAsync(q => q
                    .Where(e => e.StudentId == model.StudentId &&
                                distinctCourseIds.Contains(e.CourseId)));

                var existingCourseIds = existing.Select(e => e.CourseId).ToHashSet();

                var newEnrollments = distinctCourseIds
                    .Where(courseId => !existingCourseIds.Contains(courseId))
                    .Select(courseId => new Enrollment
                    {
                        StudentId = model.StudentId,
                        CourseId = courseId,
                        EnrollmentDate = DateTime.UtcNow // ✅ UTC not local time
                    }).ToList();

                if (!newEnrollments.Any())
                    return ServiceResult<List<Enrollment>>.Fail(
                        "This student is already enrolled in the selected course(s).");

                await _unitOfWork.Enrollments.AddRangeAsync(newEnrollments);
                await _unitOfWork.CompleteAsync();

                _logger.LogInfo($"Enrollments created for StudentId: {model.StudentId}");
                return ServiceResult<List<Enrollment>>.Ok(newEnrollments, "Enrollments created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding enrollments for StudentId: {model.StudentId}", ex);
                return ServiceResult<List<Enrollment>>.Fail("An error occurred while saving enrollments.");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int studentId, int courseId)
        {
            // FindAsync hits DB with WHERE — no full table scan
            var enrollment = await _unitOfWork.Enrollments.FindAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return ServiceResult<bool>.Fail("Enrollment not found.");

            try
            {
                _unitOfWork.Enrollments.Delete(enrollment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInfo($"Enrollment deleted. StudentId: {studentId}, CourseId: {courseId}");
                return ServiceResult<bool>.Ok(true, "Enrollment deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting enrollment. StudentId: {studentId}, CourseId: {courseId}", ex);
                return ServiceResult<bool>.Fail("An error occurred while deleting the enrollment.");
            }
        }

        private async Task<ServiceResult<bool>> ValidateBeforeSaveAsync(EnrollmentCreateVM model)
        {
            if (model.StudentId <= 0)
                return ServiceResult<bool>.Fail("Please select a valid student.");

            if (model.CourseIds == null || !model.CourseIds.Any())
                return ServiceResult<bool>.Fail("Please select at least one course.");

            var student = await _unitOfWork.Students.GetByIdAsync(model.StudentId);
            if (student == null)
                return ServiceResult<bool>.Fail("Selected student does not exist.");

            var courses = await _unitOfWork.Courses.GetAllAsync();
            var validCourseIds = courses.Select(c => c.Id).ToHashSet();
            var invalidCourseIds = model.CourseIds.Where(id => !validCourseIds.Contains(id)).ToList();

            if (invalidCourseIds.Any())
                return ServiceResult<bool>.Fail("One or more selected courses are invalid.");

            return ServiceResult<bool>.Ok(true);
        }
    }
}
