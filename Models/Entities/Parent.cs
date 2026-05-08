using Microsoft.AspNetCore.Identity;

namespace SchoolApp.Models.Entities
{
    public class Parent
    {
        public int Id { get; set; }

        // Parent full name
        public string FullName { get; set; } = string.Empty;

        // Parent phone number
        public string PhoneNumber { get; set; } = string.Empty;

        // Optional email for parent contact
        public string? Email { get; set; }

        // Optional address
        public string? Address { get; set; }

        // Optional link to Identity user
        // Later, if parent logs in, we connect this Parent profile to ApplicationUser/ApplicationUser
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        // One parent can have many students
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}