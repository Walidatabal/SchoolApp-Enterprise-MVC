// Models/Entities/Department.cs
using SchoolApp.Models.Common;

namespace SchoolApp.Models.Entities
{
    // ✅ ADDED: inherits BaseEntity
    // WHY: Same reason as Teacher — Department had its own manual Id
    //      and no audit fields. Now consistent with every other entity.
    public class Department : BaseEntity
    {
        // ✅ REMOVED: public int Id { get; set; } — now comes from BaseEntity

        public string Name { get; set; } = string.Empty;
    }
}