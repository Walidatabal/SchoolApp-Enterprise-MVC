using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models;

namespace SchoolApp.Configurations
{
    public class TeacherCourseConfiguration : IEntityTypeConfiguration<TeacherCourse>
    {
        public void Configure(EntityTypeBuilder<TeacherCourse> builder)
        {
            // Composite Primary Key
            builder.HasKey(tc => new { tc.TeacherId, tc.CourseId });

            // Relationship: TeacherCourse -> Teacher
            builder.HasOne(tc => tc.Teacher)
                   .WithMany(t => t.TeacherCourses)
                   .HasForeignKey(tc => tc.TeacherId)
                   .OnDelete(DeleteBehavior.Cascade);  // it is used 

            // Relationship: TeacherCourse -> Course
            builder.HasOne(tc => tc.Course)
                   .WithMany(c => c.TeacherCourses)
                   .HasForeignKey(tc => tc.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Optional: table name
            builder.ToTable("TeacherCourses");
        }
    }
}