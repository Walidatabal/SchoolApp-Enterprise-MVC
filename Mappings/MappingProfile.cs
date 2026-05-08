using AutoMapper;
using SchoolApp.DTOs;
using SchoolApp.Models.Entities;
using SchoolApp.ViewModels.Departments;
using SchoolApp.ViewModels.Students;
using SchoolApp.ViewModels.Teachers;

namespace SchoolApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =========================
            // Student mappings
            // =========================
            CreateMap<Student, StudentDto>();
            // Entity -> DTO

            CreateMap<StudentCreateVM, Student>();
            // Create VM -> Entity

            CreateMap<StudentEditVM, Student>();
            // Edit VM -> Entity

            // =========================
            // Teacher mappings
            // =========================
            CreateMap<Teacher, TeacherDto>();
            // Entity -> DTO

            CreateMap<TeacherCreateVM, Teacher>();
            // Create VM -> Entity

            CreateMap<TeacherEditVM, Teacher>();
            // Edit VM -> Entity

            // =========================
            // Department mappings
            // =========================
            CreateMap<Department, DepartmentDto>();
            // Entity -> DTO

            CreateMap<DepartmentCreateVM, Department>();
            // Create VM -> Entity

            CreateMap<DepartmentEditVM, Department>();
            // Edit VM -> Entity

            // =========================
            // Course mappings
            // =========================
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Teachers, opt => opt.MapFrom(src =>
                    src.TeacherCourses
                        .Where(tc => tc.Teacher != null)
                        .Select(tc => tc.Teacher!.Name)
                        .ToList()));
            // Entity -> DTO (flattens TeacherCourses to a list of names)

            // =========================
            // Enrollment mappings
            // =========================
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                    src.Student != null ? src.Student.Name : "N/A"))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src =>
                    src.Course != null ? src.Course.Name : "N/A"));
            // Entity -> DTO (flattens navigation properties)
        }
    }
}