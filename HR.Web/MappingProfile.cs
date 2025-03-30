using AutoMapper;
using HR.Core.DTOs.EmployeeDTOs;
using HR.Core.Entities;

namespace HR.Web;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeResponseDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));
    }
}