using SchoolApp.Common;
using SchoolApp.Models.Entities;
using SchoolApp.Repositories.Interfaces;
using SchoolApp.Services.Interfaces;

namespace SchoolApp.Services.Implementations
{
    public class ParentService : IParentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all parents
        public async Task<IEnumerable<Parent>> GetAllAsync()
        {
            return await _unitOfWork.Parents.GetAllAsync();
        }

        // Get parent by Id
        public async Task<Parent?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _unitOfWork.Parents.GetByIdAsync(id);
        }


        public async Task<int> GetCountAsync()
        {
            var parents = await _unitOfWork.Parents.GetAllAsync();

            return parents.Count();
        }
        // Get parent profile by linked Identity UserId
        public async Task<Parent?> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var parents = await _unitOfWork.Parents.GetAllAsync();

            return parents.FirstOrDefault(p => p.UserId == userId);
        }

        // Add new parent
        public async Task<ServiceResult<Parent>> AddAsync(Parent parent)
        {
            if (parent == null)
                return ServiceResult<Parent>.Fail("Parent data is required.");

            if (string.IsNullOrWhiteSpace(parent.FullName))
                return ServiceResult<Parent>.Fail("Parent full name is required.");

            if (string.IsNullOrWhiteSpace(parent.PhoneNumber))
                return ServiceResult<Parent>.Fail("Parent phone number is required.");

            await _unitOfWork.Parents.AddAsync(parent);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<Parent>.Ok(parent, "Parent created successfully.");
        }

        // Update existing parent
        public async Task<ServiceResult<Parent>> UpdateAsync(Parent parent)
        {
            if (parent == null)
                return ServiceResult<Parent>.Fail("Parent data is required.");

            var existingParent = await _unitOfWork.Parents.GetByIdAsync(parent.Id);

            if (existingParent == null)
                return ServiceResult<Parent>.Fail("Parent not found.");

            existingParent.FullName = parent.FullName;
            existingParent.PhoneNumber = parent.PhoneNumber;
            existingParent.Email = parent.Email;
            existingParent.Address = parent.Address;
            existingParent.UserId = parent.UserId;

            _unitOfWork.Parents.Update(existingParent);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<Parent>.Ok(existingParent, "Parent updated successfully.");
        }

        // Delete parent
        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            if (id <= 0)
                return ServiceResult<bool>.Fail("Invalid parent id.");

            var parent = await _unitOfWork.Parents.GetByIdAsync(id);

            if (parent == null)
                return ServiceResult<bool>.Fail("Parent not found.");

            _unitOfWork.Parents.Delete(parent);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<bool>.Ok(true, "Parent deleted successfully.");
        }
    }
}