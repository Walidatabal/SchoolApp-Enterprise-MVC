using FluentValidation;
using SchoolApp.ViewModels.Parents;

namespace SchoolApp.Validators.Parents
{
    public class ParentEditValidator : AbstractValidator<ParentEditVM>
    {
        public ParentEditValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Parent name is required.")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email address.");

            RuleFor(x => x.Address)
                .MaximumLength(250);
        }
    }
}