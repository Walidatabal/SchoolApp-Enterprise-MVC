using SchoolApp.Common;
using SchoolApp.Models;
using SchoolApp.Models.Entities;
using SchoolApp.ViewModels.TeacherCourses;

namespace SchoolApp.Services.Interfaces
{
    public interface ITeacherCourseService
    {
        // Get all teacher-course relations
        Task<IEnumerable<TeacherCourse>> GetAllAsync();

        // Get selected courses for teacher
        Task<List<int>> GetCourseIdsByTeacherIdAsync(int teacherId);

        // Assign courses to teacher
        Task<ServiceResult> AssignCoursesToTeacherAsync(
            int teacherId,
            List<int> courseIds);

        // Logging test

        Task<AssignCoursesToTeacherVM?> GetEditVMAsync(int teacherId, int courseId);

        Task<ServiceResult<bool>> UpdateAsync(AssignCoursesToTeacherVM vm);

        Task<TeacherCourseListItemVM?> GetByIdsAsync(int teacherId, int courseId);

        Task<ServiceResult<bool>> DeleteAsync(int teacherId, int courseId);



        void Test();


    }
}