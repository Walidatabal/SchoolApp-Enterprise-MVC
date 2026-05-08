// Models/Entities/Teacher.cs
using SchoolApp.Models.Common;


namespace SchoolApp.Models.Entities
{
    // ✅ ADDED: inherits BaseEntity
    // WHY: Teacher was the only entity (besides Department) that defined
    //      its own Id manually and had NO CreatedAt/UpdatedAt.
    //      All entities must be consistent — BaseEntity gives us:
    //        Id, CreatedAt (auto UTC), UpdatedAt (set on update)
    public class Teacher : BaseEntity
    {
        // ✅ REMOVED: public int Id { get; set; } — now comes from BaseEntity

        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();

        public string? UserId { get; set; }
        // Link this teacher profile to Identity user

        public string? Location { get; set; }

      
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    }
}

