using FluentValidation;
using SchoolApp.ViewModels.Enrollments;

namespace SchoolApp.Validators.Enrollments
{
    public class EnrollmentCreateValidator : AbstractValidator<EnrollmentCreateVM>
    {
        public EnrollmentCreateValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Please select a student.");
            // Ensure student is selected

            RuleFor(x => x.CourseIds)
                .NotEmpty().WithMessage("Please select at least one course.");
            // Ensure at least one course is selected
        }
    }
}