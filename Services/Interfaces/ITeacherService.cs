using SchoolApp.Common;
using SchoolApp.Models.Entities;
using SchoolApp.ViewModels.Teachers;

namespace SchoolApp.Services.Interfaces
{
    // Teacher Service Contract
    //
    // Defines all business operations related to Teachers.
    // Controllers communicate with this interface instead of
    // directly accessing repositories or DbContext.
    //
    // WHY?
    // - Better architecture
    // - Easier testing
    // - Loose coupling
    // - Centralized business logic
    public interface ITeacherService
    {
        // =========================
        // GET ALL TEACHERS
        // =========================
        // Example:
        // await _teacherService.GetAllAsync();
        //
        // Returns all teachers from database
        Task<IEnumerable<Teacher>> GetAllAsync();

        // =========================
        // GET TEACHER BY ID
        // =========================
        // Example:
        // await _teacherService.GetByIdAsync(5);
        //
        // Returns teacher or null if not found
        Task<Teacher?> GetByIdAsync(int id);

        // =========================
        // GET TEACHER BY USER ID
        // =========================
        // Used to link Identity user with Teacher profile
        //
        // Example:
        // await _teacherService.GetByUserIdAsync(userId);
        Task<Teacher?> GetByUserIdAsync(string userId);

        // =========================
        // CHECK SYSTEM ACCESS
        // =========================
        // Verifies if teacher is approved and allowed
        // to access protected parts of the system.
        //
        // Example:
        // await _teacherService.CanAccessSystemAsync(userId);
        Task<bool> CanAccessSystemAsync(string userId);

        // =========================
        // CREATE TEACHER
        // =========================
        // Adds new teacher to database
        //
        // Returns ServiceResult with success/failure message
        Task<ServiceResult<Teacher>> AddAsync(Teacher teacher);

        // =========================
        // UPDATE TEACHER
        // =========================
        // Updates teacher information
        //
        // IMPORTANT:
        // Should NOT reset approval status.
        Task<ServiceResult<Teacher>> UpdateAsync(Teacher teacher);

        // =========================
        // DELETE TEACHER
        // =========================
        // Deletes teacher and related records if needed
        //
        // Example:
        // await _teacherService.DeleteAsync(id);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        Task<int> GetCountAsync();

        Task<int> GetPendingTeachersCountAsync();
        Task<IEnumerable<PendingTeacherVM>> GetPendingAsync();
        Task<ServiceResult<bool>> ApproveTeacherAsync(int teacherId);
        Task<ServiceResult<bool>> RejectTeacherAsync(int teacherId);

    }
}