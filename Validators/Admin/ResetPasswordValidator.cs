using FluentValidation;
using SchoolApp.ViewModels.Account;
using SchoolApp.ViewModels.Admin;

namespace SchoolApp.Validators.Admin
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordVM>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User id is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Password and Confirm Password must match.");
        }
    }
}