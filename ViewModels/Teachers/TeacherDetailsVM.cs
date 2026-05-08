using SchoolApp.Models.Common;

namespace SchoolApp.ViewModels.Teachers
{
    public class TeacherDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public ApprovalStatus Status { get; set; }
    }
}