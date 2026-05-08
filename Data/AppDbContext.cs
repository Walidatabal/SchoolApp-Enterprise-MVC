// Data/AppDbContext.cs
using SchoolApp.Models;
using SchoolApp.Models.Common;
using SchoolApp.Models.Identity;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    // ✅ ADDED: override SaveChanges to auto-set audit fields
    // WHY: BaseEntity has CreatedAt and UpdatedAt but they were never
    //      being set automatically. Every service had to remember to set them
    //      manually — which nobody was doing. UpdatedAt was always null.
    //
    //      By overriding SaveChanges here, EF Core sets the timestamps
    //      automatically on every Add or Update — no service needs to touch them.
    //
    //      This is the standard .NET pattern for audit fields.
    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<TeacherCourse> TeacherCourses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ClassRoom> ClassRooms { get; set; }
    public DbSet<Parent> Parents { get; set; }
}