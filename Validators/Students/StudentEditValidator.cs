using FluentValidation;
using SchoolApp.ViewModels.Students;

public class StudentEditValidator : AbstractValidator<StudentEditVM>
{
    public StudentEditValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty(); // Ensure we know which record to update

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.Today); // Must be a valid past date
    }
}