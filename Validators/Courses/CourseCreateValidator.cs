using FluentValidation;
using SchoolApp.ViewModels.Courses;

namespace SchoolApp.Validators.Courses
{
    public class CourseCreateValidator : AbstractValidator<CourseCreateVM>  // It used to 
    {
        public CourseCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters.");

            RuleFor(x => x.TeacherIds)
                .NotEmpty().WithMessage("Please select at least one teacher.");
        }
    }
}