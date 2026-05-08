using SchoolApp.Common;
using SchoolApp.Models.Entities;
using SchoolApp.ViewModels.Students;

namespace SchoolApp.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<IEnumerable<Student>> GetAllWithParentsAsync();

        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByUserIdAsync(string userId);
        Task<Student?> GetByIdWithParentAsync(int id);

        Task<int> GetCountAsync();

        Task<ServiceResult<Student>> AddAsync(Student student);
        Task<ServiceResult<Student>> UpdateAsync(Student student);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // ViewModel builders for MVC UI
        Task<StudentCreateVM> BuildCreateVMAsync();
        Task<StudentCreateVM> RebuildCreateVMAsync(StudentCreateVM vm);

        Task<StudentEditVM?> BuildEditVMAsync(int id);
        Task<StudentEditVM> RebuildEditVMAsync(StudentEditVM vm);

        Task<StudentDetailsVM?> BuildDetailsVMAsync(int id);
    }
}