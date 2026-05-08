using FluentValidation;
using SchoolApp.ViewModels.Account;

namespace SchoolApp.Validators.Account
{
    public class ForgotPasswordVMValidator : AbstractValidator<ForgotPasswordVM>
    {
        public ForgotPasswordVMValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please enter a valid email.");
        }
    }
}