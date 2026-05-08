using SchoolApp.Common;
using SchoolApp.Models.Entities;

namespace SchoolApp.Services.Interfaces
{
    public interface IParentService
    {
        Task<IEnumerable<Parent>> GetAllAsync();

        Task<Parent?> GetByIdAsync(int id);

        Task<Parent?> GetByUserIdAsync(string userId);

        Task<ServiceResult<Parent>> AddAsync(Parent parent);

        Task<ServiceResult<Parent>> UpdateAsync(Parent parent);

        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<int> GetCountAsync();

        
    }
}