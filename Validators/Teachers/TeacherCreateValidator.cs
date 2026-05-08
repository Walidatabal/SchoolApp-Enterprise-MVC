using FluentValidation;
using SchoolApp.ViewModels.Teachers;

namespace SchoolApp.Validators.Teachers
{
    public class TeacherCreateValidator : AbstractValidator<TeacherCreateVM>
    {
        public TeacherCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Teacher name is required.")
                .MaximumLength(100).WithMessage("Teacher name cannot exceed 100 characters.");
            // Validate teacher name

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.")
                .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");
            // Validate specialization
        }
    }
}