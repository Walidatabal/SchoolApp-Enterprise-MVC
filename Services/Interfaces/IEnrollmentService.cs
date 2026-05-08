using SchoolApp.ViewModels.Enrollments;

namespace SchoolApp.Services.Interfaces
{
    // Enrollment Service Contract
    // Handles Student ↔ Course relationship.
    public interface IEnrollmentService
    {
        // Get all enrollments with Student and Course details
        Task<IEnumerable<Enrollment>> GetAllAsync();

        // Get one enrollment using composite key
        // StudentId + CourseId
        Task<Enrollment?> GetByIdAsync(int studentId, int courseId);

        // Add multiple course enrollments for one student
        Task<ServiceResult<List<Enrollment>>> AddRangeAsync(
            EnrollmentCreateVM model);

        // Delete one enrollment relation
        Task<ServiceResult<bool>> DeleteAsync(
            int studentId,
            int courseId);
    }
}