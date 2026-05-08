namespace SchoolApp.Models.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }

        // ✅ IMPROVED: Use DateTime.UtcNow instead of DateTime.Now
        // WHY: DateTime.Now returns the server's LOCAL time, which causes bugs
        //      if your server or database is in a different timezone than your users.
        //      DateTime.UtcNow always returns UTC — consistent everywhere in the world.
        //      Best practice: always store UTC in the database, convert to local
        //      time only when displaying to the user.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        // Nullable (?) because it's only set when the record is updated,
        // not when it's first created
    }
}