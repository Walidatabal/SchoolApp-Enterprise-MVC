using FluentValidation;
using SchoolApp.ViewModels.Teachers;

namespace SchoolApp.Validators.Teachers
{
    public class TeacherEditValidator : AbstractValidator<TeacherEditVM>
    {
        public TeacherEditValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Teacher Id is required.");
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