using System.Linq.Expressions;

namespace SchoolApp.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>> include = null);

        Task<T?> GetByIdAsync(int id);  // Assuming all entities have an int Id property

        Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IQueryable<T>> include = null);

        // Used for searching one record by condition
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        // Used for searching multiple records by condition — avoids loading all rows into memory
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);

        // Used for adding multiple records
        Task AddRangeAsync(IEnumerable<T> entities);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> CountAsync();

        // =========================
        // GET FIRST MATCHING ENTITY
        // =========================
        // WHY:
        // Retrieves a single entity that matches a condition.
        //
        // Example:
        // Get student with Id = 5
        //
        // Also supports Include() for related entities.
        //
        // Example:
        // Load Student WITH Parent entity
        //
        // BENEFITS:
        // - Avoids loading all records into memory
        // - Executes efficient SQL query using WHERE + TOP(1)
        // - Supports eager loading with Include()
        // - Enterprise repository pattern
        //
        // PARAMETERS:
        //
        // predicate:
        // Condition used to filter data.
        // Example:
        // s => s.Id == id
        //
        // include:
        // Optional Include() chain for related entities.
        // Example:
        // q => q.Include(s => s.Parent)
        //
        // RETURNS:
        // First matching entity or null if not found.
        //
        // EXAMPLE USAGE:
        //
        // await _unitOfWork.Students.FirstOrDefaultAsync(
        //     s => s.Id == id,
        //     q => q.Include(s => s.Parent)
        // );
        //
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null);
    }
}