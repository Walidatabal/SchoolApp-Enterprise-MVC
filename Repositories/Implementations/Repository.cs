using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SchoolApp.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        // Represents the table for the entity
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;

            // Get the table dynamically based on entity type
            _dbSet = _context.Set<T>();
        }

        // =========================================
        // GET ALL WITHOUT INCLUDE
        // =========================================
        // Returns all records from the table
        // Example:
        // var students = await _unitOfWork.Students.GetAllAsync();
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // =========================================
        // GET ALL WITH INCLUDE
        // =========================================
        // Allows loading related entities using Include()
        //
        // Example:
        // await _unitOfWork.Students.GetAllAsync(
        //      q => q.Include(s => s.Parent)
        // );
        //
        // Another Example:
        // await _unitOfWork.Courses.GetAllAsync(
        //      q => q.Include(c => c.TeacherCourses)
        //            .ThenInclude(tc => tc.Teacher)
        // );
        public async Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            // Start query from table
            IQueryable<T> query = _dbSet;

            // Apply Include if provided
            if (include != null)
            {
                query = include(query);
            }

            // Execute query and return results
            return await query.ToListAsync();
        }

        // =========================================
        // GET BY ID WITHOUT INCLUDE
        // =========================================
        // Finds one entity by primary key
        //
        // Example:
        // var student = await _unitOfWork.Students.GetByIdAsync(1);
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // =========================================
        // GET BY ID WITH INCLUDE
        // =========================================
        // Finds one entity with related entities
        //
        // Example:
        // await _unitOfWork.Students.GetByIdAsync(
        //      1,
        //      q => q.Include(s => s.Parent)
        // );
        public async Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            // Start query from table
            IQueryable<T> query = _dbSet;

            // Apply Include if provided
            if (include != null)
            {
                query = include(query);
            }

            // Search dynamically by Id property
            // EF.Property allows generic access to entity properties
            return await query.FirstOrDefaultAsync(
                e => EF.Property<int>(e, "Id") == id
            );
        }

        // =========================================
        // ADD NEW ENTITY
        // =========================================
        // Adds new entity to DbContext
        //
        // Note:
        // Changes are NOT saved until CompleteAsync()
        //
        // Example:
        // await _unitOfWork.Students.AddAsync(student);
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // =========================================
        // UPDATE ENTITY
        // =========================================
        // Marks entity as modified
        //
        // Example:
        // _unitOfWork.Students.Update(student);
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // =========================================
        // DELETE ENTITY
        // =========================================
        // Removes entity from DbContext
        //
        // Example:
        // _unitOfWork.Students.Delete(student);
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // =========================================
        // FIND ONE BY CONDITION
        // =========================================
        // Example:
        // await _unitOfWork.Students.FindAsync(s => s.UserId == userId);
        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }


        // =========================================
        // ADD MULTIPLE ENTITIES
        // =========================================
        // Example:
        // await _unitOfWork.Enrollments.AddRangeAsync(enrollments);
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<T?> FirstOrDefaultAsync
            (Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}