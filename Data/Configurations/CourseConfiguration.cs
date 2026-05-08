using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // ✅ REMOVED: any HasOne/WithMany/HasForeignKey for TeacherId
            // WHY: The old one-to-many configuration (Course → Teacher via TeacherId)
            //      no longer exists. The relationship is now managed entirely
            //      through TeacherCourseConfiguration using the join table.
            //      Leaving old FK configuration here would cause EF Core to
            //      try to create a relationship that no longer exists in the model
            //      and throw a migration error.

            // ✅ The many-to-many relationship is configured in:
            //    TeacherCourseConfiguration.cs — that file handles the join table
            //    No relationship config needed here in CourseConfiguration

            // Existing configurations (لو عندك)

            // =========================
            // ClassRoom Relationship
            // =========================
            builder.HasOne(c => c.ClassRoom)
                   .WithMany(cr => cr.Courses)
                   .HasForeignKey(c => c.ClassRoomId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}