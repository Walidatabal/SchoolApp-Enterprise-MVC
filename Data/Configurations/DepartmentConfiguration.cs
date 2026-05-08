using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            // Primary key

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            // Required field with max length
        }
    }
}