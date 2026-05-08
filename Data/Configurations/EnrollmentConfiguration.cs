using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");
            // Set table name

            builder.HasKey(e => new { e.StudentId, e.CourseId });
            // Composite Key → prevents duplicate enrollment for same student/course

            builder.Property(e => e.EnrollmentDate)
                .IsRequired();
            // Enrollment date is required

            builder.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);
            // Student 1 → many Enrollments

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);
            // Course 1 → many Enrollments
        }
    }
}