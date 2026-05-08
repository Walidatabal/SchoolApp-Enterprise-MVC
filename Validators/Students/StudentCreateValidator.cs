using FluentValidation;
using SchoolApp.ViewModels.Students;

namespace SchoolApp.Validators.Students
{
    public class StudentCreateValidator : AbstractValidator<StudentCreateVM>
    {
        public StudentCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Student name is required.")
                .MaximumLength(100).WithMessage("Student name cannot exceed 100 characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.");
        }
    }
}