

using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolApp.ViewModels.Students;

namespace SchoolApp.Services.Implementations
{
    // Handles all student business logic
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger _logger;

        // Constructor Injection
        public StudentService(
            IUnitOfWork unitOfWork,
            IAppLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // =========================
        // GET ALL STUDENTS
        // =========================
        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _unitOfWork.Students.GetAllAsync();
        }

        public async Task<int> GetCountAsync()
        {
            var students = await _unitOfWork.Students.CountAsync();

            return students;
        }

        // =========================
        // GET STUDENT BY ID
        // =========================
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Students.GetByIdAsync(id);
        }

        // =========================
        // GET STUDENT BY USER ID
        // =========================
        // Used to connect Identity user with Student profile
        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _unitOfWork.Students
                .FindAsync(s => s.UserId == userId);
        }

        // =========================
        // CREATE STUDENT
        // =========================
        public async Task<ServiceResult<Student>> AddAsync(Student student)
        {
            // Validate data first
            var validation = ValidateStudent(student);

            if (!validation.Success)
                return ServiceResult<Student>.Fail(validation.Message);

            try
            {
                _logger.LogInfo(
                    $"Creating student: {student.Name}");

                // Add student to DbContext
                await _unitOfWork.Students.AddAsync(student);

                // Save changes to database
                await _unitOfWork.CompleteAsync();

                return ServiceResult<Student>.Ok(
                    student,
                    "Student created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while creating the student.",
                    ex);

                return ServiceResult<Student>.Fail(
                    "An error occurred while saving the student.");
            }
        }

        // =========================
        // UPDATE STUDENT
        // =========================
        public async Task<ServiceResult<Student>> UpdateAsync(Student student)
        {

            var validation = ValidateStudent(student);

            if (!validation.Success)
                return ServiceResult<Student>.Fail(validation.Message);

            try
            {
                // Get existing student first
                var existingStudent =
                    await _unitOfWork.Students.GetByIdAsync(student.Id);

                if (existingStudent == null)
                    return ServiceResult<Student>.Fail(
                        "Student not found.");

                // Update editable fields only
                existingStudent.Name = student.Name;
                existingStudent.BirthDate = student.BirthDate;
                existingStudent.ParentId = student.ParentId;

                // Mark entity as modified
                _unitOfWork.Students.Update(existingStudent);

                // Save changes
                await _unitOfWork.CompleteAsync();

                _logger.LogInfo(
                    $"Student updated successfully. ID: {student.Id}");

                return ServiceResult<Student>.Ok(
                    existingStudent,
                    "Student updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while updating the student.",
                    ex);

                return ServiceResult<Student>.Fail(
                    "An error occurred while updating the student.");
            }
        }

        // =========================
        // DELETE STUDENT
        // =========================
        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                // Get student from database
                var student =
                    await _unitOfWork.Students.GetByIdAsync(id);

                if (student == null)
                    return ServiceResult<bool>.Fail(
                        "Student not found.");

                // Load only this student's enrollments — no full table scan
                var studentEnrollments =
                    await _unitOfWork.Enrollments.FindAllAsync(e => e.StudentId == id);

                // Delete enrollments first
                foreach (var enrollment in studentEnrollments)
                {
                    _unitOfWork.Enrollments.Delete(enrollment);
                }

                // Delete student
                _unitOfWork.Students.Delete(student);

                // Save all changes
                await _unitOfWork.CompleteAsync();

                _logger.LogInfo(
                    $"Student deleted successfully. ID: {id}");

                return ServiceResult<bool>.Ok(
                    true,
                    "Student deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error occurred while deleting the student.",
                    ex);

                return ServiceResult<bool>.Fail(
                    "An error occurred while deleting the student.");
            }
        }

        // =========================
        // VALIDATE STUDENT
        // =========================
        private ServiceResult<bool> ValidateStudent(Student student)
        {
            if (student == null)
                return ServiceResult<bool>.Fail(
                    "Student data is required.");

            if (string.IsNullOrWhiteSpace(student.Name))
                return ServiceResult<bool>.Fail(
                    "Student name is required.");

            if (student.BirthDate == default)
                return ServiceResult<bool>.Fail(
                    "Birth date is required.");

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<IEnumerable<Student>> GetAllWithParentsAsync() // Example of eager loading related entities
        {
            return await _unitOfWork.Students.GetAllAsync(q => q.Include(s => s.Parent));
        }

        public async Task<Student?> GetByIdWithParentAsync(int id)
        {
            /*It means SELECT TOP(1) ...
                       FROM Students
                       LEFT JOIN Parents ...
                       WHERE Students.Id = @id*/
            return await _unitOfWork.Students.FirstOrDefaultAsync(
            s => s.Id == id,
            q => q.Include(s => s.Parent)
            );
        }


    

    // =========================
// BUILD CREATE VM
// =========================
public async Task<StudentCreateVM> BuildCreateVMAsync()
        {
            return new StudentCreateVM
            {
                Parents = await BuildParentsSelectListAsync()
            };
        }

        // =========================
        // REBUILD CREATE VM
        // =========================
        public async Task<StudentCreateVM> RebuildCreateVMAsync(StudentCreateVM vm)
        {
            vm.Parents = await BuildParentsSelectListAsync();
            return vm;
        }

        // =========================
        // BUILD EDIT VM
        // =========================
        public async Task<StudentEditVM?> BuildEditVMAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);

            if (student == null)
                return null;

            return new StudentEditVM
            {
                Id = student.Id,
                Name = student.Name,
                BirthDate = student.BirthDate,
                ParentId = student.ParentId,
                Parents = await BuildParentsSelectListAsync()
            };
        }

        // =========================
        // REBUILD EDIT VM
        // =========================
        public async Task<StudentEditVM> RebuildEditVMAsync(StudentEditVM vm)
        {
            vm.Parents = await BuildParentsSelectListAsync();
            return vm;
        }

        // =========================
        // BUILD DETAILS VM
        // =========================
        public async Task<StudentDetailsVM?> BuildDetailsVMAsync(int id)
        {
            var student = await GetByIdWithParentAsync(id);

            if (student == null)
                return null;

            return new StudentDetailsVM
            {
                Id = student.Id,
                Name = student.Name,
                BirthDate = student.BirthDate,
                ParentName = student.Parent != null
                    ? student.Parent.FullName
                    : "No Parent"
            };
        }

        // =========================
        // PRIVATE DROPDOWN BUILDER
        // =========================
        private async Task<IEnumerable<SelectListItem>> BuildParentsSelectListAsync()
        {
            var parents = await _unitOfWork.Parents.GetAllAsync();

            return parents.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FullName
            }).ToList();
        }
    }
}