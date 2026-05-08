using SchoolApp.Data;
using SchoolApp.Models;
using SchoolApp.Models.Entities;
using SchoolApp.Repositories.Interfaces;

namespace SchoolApp.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<Student> Students { get; private set; }
        public IRepository<Teacher> Teachers { get; private set; }
        public IRepository<Course> Courses { get; private set; }
        public IRepository<Enrollment> Enrollments { get; private set; }
        public IRepository<TeacherCourse> TeacherCourses { get; private set; }
        public IRepository<Department> Departments { get; private set; }
        public IRepository<ClassRoom> ClassRooms { get; private set; }
        public IRepository<Parent> Parents { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Students = new Repository<Student>(_context);
            Teachers = new Repository<Teacher>(_context);
            Courses = new Repository<Course>(_context);
            Enrollments = new Repository<Enrollment>(_context);
            TeacherCourses = new Repository<TeacherCourse>(_context);
            Departments = new Repository<Department>(_context);
            ClassRooms = new Repository<ClassRoom>(_context);
            Parents = new Repository<Parent>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}