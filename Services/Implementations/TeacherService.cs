using SchoolApp.Common;
using SchoolApp.Common.Extensions;
using SchoolApp.Models.Common;
using SchoolApp.Models.Entities;
using SchoolApp.Repositories.Interfaces;
using SchoolApp.Services.Interfaces;
using SchoolApp.ViewModels.Teachers;


namespace SchoolApp.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger _logger;

        public TeacherService(IUnitOfWork unitOfWork, IAppLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _unitOfWork.Teachers.GetAllAsync();
        }

        public async Task<int> GetCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.CountAsync();  // for better performance, use CountAsync() instead of GetAllAsync() + Count()

            return teachers;   // to make it clear we can use return await _unitOfWork.Teachers.CountAsync(); 
        }

        public async Task<int> GetPendingTeachersCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync();

            return teachers.Count(t => t.Status == ApprovalStatus.Pending);
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Teachers.GetByIdAsync(id);
        }

        public async Task<Teacher?> GetByUserIdAsync(string userId)
        {
            return await _unitOfWork.Teachers
                .FindAsync(t => t.UserId == userId);
        }

        public async Task<bool> CanAccessSystemAsync(string userId)
        {
            var teacher = await GetByUserIdAsync(userId);

            return teacher != null && teacher.Status.IsApproved();
        }

        public async Task<ServiceResult<Teacher>> AddAsync(Teacher teacher)
        {
            var validation = ValidateTeacher(teacher);

            if (!validation.Success)
                return ServiceResult<Teacher>.Fail(validation.Message);

            try
            {
                _logger.LogInfo($"Creating teacher: {teacher.Name}");

                await _unitOfWork.Teachers.AddAsync(teacher);
                await _unitOfWork.CompleteAsync();

                return ServiceResult<Teacher>.Ok(
                    teacher,
                    "Teacher created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while saving the teacher.",
                    ex);

                return ServiceResult<Teacher>.Fail(
                    "An error occurred while saving the teacher.");
            }
        }

        public async Task<ServiceResult<Teacher>> UpdateAsync(Teacher teacher)
        {
            var validation = ValidateTeacher(teacher);

            if (!validation.Success)
                return ServiceResult<Teacher>.Fail(validation.Message);

            try
            {
                var existingTeacher =
                    await _unitOfWork.Teachers.GetByIdAsync(teacher.Id);

                if (existingTeacher == null)
                    return ServiceResult<Teacher>.Fail("Teacher not found.");

                existingTeacher.Name = teacher.Name;
                existingTeacher.Specialization = teacher.Specialization;

                // Do NOT reset Status here.
                // Keep Pending / Approved / Rejected as existing value.

                _unitOfWork.Teachers.Update(existingTeacher);
                await _unitOfWork.CompleteAsync();

                return ServiceResult<Teacher>.Ok(
                    existingTeacher,
                    "Teacher updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while updating the teacher.",
                    ex);

                return ServiceResult<Teacher>.Fail(
                    "An error occurred while updating the teacher.");
            }
        }


        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);

                if (teacher == null)
                    return ServiceResult<bool>.Fail("Teacher not found.");

                // Load only this teacher's course relations — no full table scan
                var relatedTeacherCourses =
                    await _unitOfWork.TeacherCourses.FindAllAsync(tc => tc.TeacherId == id);

                foreach (var relation in relatedTeacherCourses)
                {
                    _unitOfWork.TeacherCourses.Delete(relation);
                }

                _logger.LogInfo(
                    $"Deleting teacher: {teacher.Name} (ID: {teacher.Id})");

                _unitOfWork.Teachers.Delete(teacher);
                await _unitOfWork.CompleteAsync();

                return ServiceResult<bool>.Ok(
                    true,
                    "Teacher deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while deleting the teacher.",
                    ex);

                return ServiceResult<bool>.Fail(
                    "An error occurred while deleting the teacher.");
            }
        }

        private ServiceResult<bool> ValidateTeacher(Teacher teacher)
        {
            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher data is required.");

            if (string.IsNullOrWhiteSpace(teacher.Name))
                return ServiceResult<bool>.Fail("Teacher name is required.");

            if (string.IsNullOrWhiteSpace(teacher.Specialization))
                return ServiceResult<bool>.Fail("Specialization is required.");

            return ServiceResult<bool>.Ok(true);
        }

        //public Task<IEnumerable<Teacher>> GetPendingAsync()
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<IEnumerable<PendingTeacherVM>> GetPendingAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(q => q
                .Where(t => t.Status == ApprovalStatus.Pending));

            return teachers.Select(t => new PendingTeacherVM
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Specialization = t.Specialization,
                Status = t.Status.ToString()
            }).ToList();
        }

        public async Task<ServiceResult<bool>> ApproveTeacherAsync(int teacherId)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId);

            if (teacher == null)
            {
                _logger.LogInfo($"Approve failed. TeacherId={teacherId} not found");
                return ServiceResult<bool>.Fail("Teacher not found.");
            }

            if (teacher.Status == ApprovalStatus.Approved)
            {
                _logger.LogInfo($"Approve skipped. TeacherId={teacherId} already approved");
                return ServiceResult<bool>.Fail("Teacher is already approved.");
            }

            var oldStatus = teacher.Status;

            teacher.Status = ApprovalStatus.Approved;

            _logger.LogInfo(
                $"Teacher approved. Id={teacherId}, OldStatus={oldStatus}, NewStatus={teacher.Status}");

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> RejectTeacherAsync(int teacherId)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId);

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            if (teacher.Status == ApprovalStatus.Rejected)
                return ServiceResult<bool>.Fail("Teacher is already rejected.");

            if (teacher.Status == ApprovalStatus.Approved)
                return ServiceResult<bool>.Fail("Approved teacher cannot be rejected directly.");

            teacher.Status = ApprovalStatus.Rejected;

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<bool>.Ok(true);
        }

    }
}
