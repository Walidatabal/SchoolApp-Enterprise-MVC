using FluentValidation;
using SchoolApp.ViewModels.Courses;

namespace SchoolApp.Validators.Courses
{
    public class CourseEditValidator : AbstractValidator<CourseEditVM>
    {
        public CourseEditValidator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("Course Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters.");

            RuleFor(x => x.TeacherIds)
                .NotEmpty().WithMessage("Please select at least one teacher.");
        }
    }
}