using SchoolApp.Models;
using SchoolApp.Models.Entities;

namespace SchoolApp.Repositories.Interfaces
{
    // Unit Of Work Pattern
    // Coordinates all repositories together
    //
    // WHY?
    // Instead of saving changes after every repository action,
    // we save ALL changes once using CompleteAsync().
    public interface IUnitOfWork
    {
        // Student repository
        IRepository<Student> Students { get; }  // Expose the Student repository through the Unit of Work means that any code that has access to the Unit of Work can perform operations on students without needing to know about the underlying database context or how the repositories are implemented. This promotes a cleaner separation of concerns and makes it easier to manage database transactions across multiple repositories.

        // Teacher repository
        IRepository<Teacher> Teachers { get; }

        // Course repository
        IRepository<Course> Courses { get; }

        // Enrollment repository
        IRepository<Enrollment> Enrollments { get; }

        // Teacher-Course repository
        IRepository<TeacherCourse> TeacherCourses { get; }

        // Department repository
        IRepository<Department> Departments { get; }

        // Parent repository
        IRepository<Parent> Parents { get; }
        IRepository<ClassRoom> ClassRooms { get; }


        // Save ALL pending database changes asynchronously
        //
        // Example:
        // await _unitOfWork.CompleteAsync();
        Task<int> CompleteAsync();
    }
}