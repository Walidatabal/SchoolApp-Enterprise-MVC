using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");
            // Set database table name

            builder.HasKey(t => t.Id);
            // Configure primary key

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);
            // Name is required and max length is 100

            builder.Property(t => t.Specialization)
                .IsRequired()
                .HasMaxLength(100);
            // Specialization is required and max length is 100
        }
    }
}