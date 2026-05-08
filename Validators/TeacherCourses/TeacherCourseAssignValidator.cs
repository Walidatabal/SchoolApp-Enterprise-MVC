using FluentValidation;
using SchoolApp.ViewModels.TeacherCourses;

namespace SchoolApp.Validators.TeacherCourses
{
    public class TeacherCourseAssignValidator : AbstractValidator<AssignCoursesToTeacherVM>
    {
        public TeacherCourseAssignValidator()
        {
            // Teacher must be selected
            RuleFor(x => x.TeacherId)
                .GreaterThan(0)
                .WithMessage("Please select a teacher.");

            // At least one course must be selected
            RuleFor(x => x.CourseIds)
                .NotEmpty()
                .WithMessage("Please select at least one course.");
        }
    }
}