using FluentValidation;
using SchoolApp.ViewModels.Admin;

namespace SchoolApp.Validators.Admin
{
    public class CreateUserValidator : AbstractValidator<CreateUserVM>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Password and Confirm Password must match.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Please select a role.");
        }
    }
}