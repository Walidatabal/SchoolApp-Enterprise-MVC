using AutoMapper;
using SchoolApp.DTOs;
using SchoolApp.Models.Entities;
using SchoolApp.ViewModels.Students;

namespace SchoolApp.Mappings
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            // Entity -> DTO
            CreateMap<Student, StudentDto>();

            // Create VM -> Entity
            CreateMap<StudentCreateVM, Student>();

            // Edit VM <-> Entity
            CreateMap<StudentEditVM, Student>()
                .ReverseMap();

            // Entity -> List VM
            CreateMap<Student, StudentListItemVM>()
                .ForMember(dest => dest.ParentName,
                    opt => opt.MapFrom(src =>
                        src.Parent != null
                            ? src.Parent.FullName
                            : "No Parent"));

            // Entity -> Details VM
            CreateMap<Student, StudentDetailsVM>()
                .ForMember(dest => dest.ParentName,
                    opt => opt.MapFrom(src =>
                        src.Parent != null
                            ? src.Parent.FullName
                            : "No Parent"));
        }
    }
}