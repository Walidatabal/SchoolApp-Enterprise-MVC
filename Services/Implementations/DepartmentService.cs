namespace SchoolApp.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger _logger;

        public DepartmentService(IUnitOfWork unitOfWork, IAppLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
            => await _unitOfWork.Departments.GetAllAsync();

        public async Task<Department?> GetByIdAsync(int id)
            => await _unitOfWork.Departments.GetByIdAsync(id);

        public async Task<ServiceResult<Department>> AddAsync(Department department)
        {
            var validation = ValidateDepartment(department);
            if (!validation.Success)
                return ServiceResult<Department>.Fail(validation.Message);

            try
            {
                _logger.LogInfo($"Creating department: {department.Name}");
                await _unitOfWork.Departments.AddAsync(department);
                await _unitOfWork.CompleteAsync();
                return ServiceResult<Department>.Ok(department, "Department created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while saving the department.", ex);
                return ServiceResult<Department>.Fail("An error occurred while saving the department.");
            }
        }

        public async Task<ServiceResult<Department>> UpdateAsync(
            Department department)
        {
            var validation = ValidateDepartment(department);

            if (!validation.Success)
                return ServiceResult<Department>.Fail(validation.Message);

            try
            {
                var existingDepartment =
                    await _unitOfWork.Departments.GetByIdAsync(department.Id);

                if (existingDepartment == null)
                    return ServiceResult<Department>.Fail(
                        "Department not found.");

                // Update editable fields only
                existingDepartment.Name = department.Name;

                _logger.LogInfo(
                    $"Updating department: {department.Name}");

                _unitOfWork.Departments.Update(existingDepartment);

                await _unitOfWork.CompleteAsync();

                return ServiceResult<Department>.Ok(
                    existingDepartment,
                    "Department updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error occurred while updating the department: {department.Name}",
                    ex);

                return ServiceResult<Department>.Fail(
                    "An error occurred while updating the department.");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                return ServiceResult<bool>.Fail("Department not found.");

            try
            {
                _logger.LogInfo($"Deleting department: {department.Name}");
                _unitOfWork.Departments.Delete(department);
                await _unitOfWork.CompleteAsync();
                return ServiceResult<bool>.Ok(true, "Department deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the department: {department.Name}", ex);
                return ServiceResult<bool>.Fail("An error occurred while deleting the department.");
            }
        }

        private ServiceResult<bool> ValidateDepartment(Department department)
        {
            if (department == null)
                return ServiceResult<bool>.Fail("Department data is required.");
            if (string.IsNullOrWhiteSpace(department.Name))
                return ServiceResult<bool>.Fail("Department name is required.");

            return ServiceResult<bool>.Ok(true);
        }
    }
}
