using SchoolApp.Common;
using SchoolApp.Models.Entities;

namespace SchoolApp.Services.Interfaces
{
    // Course Service Contract
    // Defines all business operations related to Courses.
    public interface ICourseService
    {
        // Get all courses with related teacher-course data if needed
        Task<IEnumerable<Course>> GetAllAsync();

        // Get single course by Id
        Task<Course?> GetByIdAsync(int id);

        // Create new course and assign selected teachers
        Task<ServiceResult<Course>> AddAsync(
            Course course,
            List<int> teacherIds);

        // Update course data and assigned teachers
        Task<ServiceResult<Course>> UpdateAsync(
            Course course,
            List<int> teacherIds);

        // Delete course and related enrollments / teacher-course relations
        Task<ServiceResult<bool>> DeleteAsync(int id);

        Task<int> GetCountAsync();
    }
}