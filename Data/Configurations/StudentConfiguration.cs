using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

// It protects the database from invalid data and ensure that the Student entity is correctly mapped to the database schema. It defines the table name, primary key, and constraints for the Name and BirthDate properties, ensuring that the database structure aligns with the Student model's requirements.


namespace SchoolApp.Data.Configurations  // Coniguration control how the Student entity maps to the database and the relationships between entities. It ensures that the database schema is correctly generated based on the Student model and its properties.
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>  // This class implements the IEntityTypeConfiguration interface, which requires us to implement the Configure method to define the configuration for the Student entity.
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students"); // Table name

            //builder.HasKey(x => x.Id); // Primary key

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.BirthDate)
                .IsRequired();

            // ✅ CreatedAt and UpdatedAt are inherited from BaseEntity
            // EF Core will automatically create columns for them in the Students table
            // You can configure them here if needed (e.g. default value), but it's optional.
            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            // This tells SQL Server to auto-fill CreatedAt with the current date
            // if no value is provided at insert time

        }
    }
}