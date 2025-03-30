using AutoMapper;
using HR.Core.DTOs.EmployeeDTOs;
using HR.Core.DTOs.LeaveRequestDTOs;
using HR.Core.Entities;

namespace HR.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeResponseDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));
        CreateMap<CreateEmployeeDto, Employee>();

        CreateMap<LeaveRequest, LeaveRequestResponseDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<CreateLeaveRequestDto, LeaveRequest>();
    }
}