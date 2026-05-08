namespace SchoolApp.Services.Interfaces
{
    // Department Service Contract
    // Handles all business operations related to Departments.
    public interface IDepartmentService
    {
        // Get all departments
        Task<IEnumerable<Department>> GetAllAsync();

        // Get one department by Id
        Task<Department?> GetByIdAsync(int id);

        // Create department
        Task<ServiceResult<Department>> AddAsync(Department department);

        // Update department
        Task<ServiceResult<Department>> UpdateAsync(Department department);

        // Delete department
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}