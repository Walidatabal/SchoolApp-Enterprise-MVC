using SchoolApp.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SchoolApp.Models.Entities
{
    public class Student : BaseEntity
    {

         //public int Id { get; set; } // Primary Key
        [Required]
        public string Name { get; set; } // اسم الطالب مطلوب  
        
         public DateTime BirthDate { get; set; } // تاريخ الميلاد

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        // Many-to-Many relation with Course via Enrollment navigation property

        //public int ClassId { get; set; }
        //public Class Class { get; set; }


        // Link this student profile to Identity user
        public string? UserId { get; set; }

        // Foreign Key to Parent
        public int? ParentId { get; set; }

        // Navigation Property
        public Parent? Parent { get; set; }
    }

    //public ICollection<Enrollment> Enrollments { get; set; }
}


