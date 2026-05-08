using SchoolApp.Models.Common;

namespace SchoolApp.Models.Entities
{
    public class Course : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string test { get; set; } = string.Empty;
        // ✅ REMOVED: public int TeacherId { get; set; }
        //
        // WHY IT WAS WRONG:
        //   TeacherId was a leftover from an earlier version of the design
        //   where each Course belonged to exactly ONE Teacher (one-to-many).
        //
        //   The design was then upgraded to many-to-many:
        //   one Course can have MULTIPLE Teachers, managed through
        //   the TeacherCourse join table.
        //
        //   But TeacherId was never removed from the entity — it became
        //   a dead column sitting in the database with no purpose.
        //
        // WHY IT'S DANGEROUS TO LEAVE IT:
        //   1. CONFUSION: any developer reading Course.cs sees TeacherId
        //      and assumes each course has ONE teacher. That contradicts
        //      the TeacherCourses collection right below it.
        //      Two conflicting designs in the same class.
        //
        //   2. DATA INCONSISTENCY: TeacherId might hold a value that
        //      doesn't match the actual teachers in TeacherCourses.
        //      You'd have two sources of truth disagreeing with each other.
        //
        //   3. DEAD COLUMN IN DB: The Courses table has a TeacherId column
        //      that is never read or written by any service or controller.
        //      It wastes storage and confuses anyone looking at the schema.
        //
        // THE CORRECT DESIGN:
        //   Course ←→ TeacherCourse ←→ Teacher
        //   The TeacherCourses collection is the ONLY source of truth
        //   for which teachers are assigned to a course.

        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        // Many-to-Many: a course can be taught by multiple teachers
        // Each row in TeacherCourse links one Course to one Teacher

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        // Many-to-Many: a course can have multiple students enrolled
        // Each row in Enrollment links one Student to one Course

        public int? ClassRoomId { get; set; }
        public ClassRoom? ClassRoom { get; set; }
    }
}