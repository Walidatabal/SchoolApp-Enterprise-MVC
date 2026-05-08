using FluentValidation;
using SchoolApp.ViewModels.Departments;

namespace SchoolApp.Validators.Departments
{
    public class DepartmentDeleteValidator : AbstractValidator<DepartmentDeleteVM>
    {
        public DepartmentDeleteValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid department id.");
        }
    }
}