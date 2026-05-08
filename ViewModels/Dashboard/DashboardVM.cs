namespace SchoolApp.ViewModels.Dashboard
{
    public class DashboardVM
    {
        public int StudentsCount { get; set; }
        // Total students count

        public int UsersCount { get; set; }
        // Total registered users count

        public int AdminsCount { get; set; }
        // Total admin users count

        public int NormalUsersCount { get; set; }
        // Total non-admin users count
        public int TeachersCount { get; set; }
        public int PendingTeachersCount { get; set; }
        public int CoursesCount { get; set; }
        public int EnrollmentsCount { get; set; }

        public int ParentsCount { get; set; }

    }
}