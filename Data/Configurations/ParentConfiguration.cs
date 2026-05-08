using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolApp.Models.Entities;

namespace SchoolApp.Data.Configurations
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Email)
                   .HasMaxLength(100);

            builder.Property(p => p.Address)
                   .HasMaxLength(250);

            // Parent -> ApplicationUser
            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Parent -> Students
            builder.HasMany(p => p.Students)
                   .WithOne(s => s.Parent)
                   .HasForeignKey(s => s.ParentId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}