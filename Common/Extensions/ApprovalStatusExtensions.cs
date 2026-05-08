using SchoolApp.Models.Common;

namespace SchoolApp.Common.Extensions
{
    public static class ApprovalStatusExtensions
    {
        public static bool IsApproved(this ApprovalStatus status)
        {
            return status == ApprovalStatus.Approved;
        }

        public static string GetTeacherStatusText(this ApprovalStatus status)
        {
            return status switch
            {
                ApprovalStatus.Pending => "Teacher - Pending Approval",
                ApprovalStatus.Rejected => "Teacher - Rejected",
                ApprovalStatus.Approved => "Teacher",
                _ => "Teacher"
            };
        }
    }
}