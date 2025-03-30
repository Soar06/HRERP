using AutoMapper;
using HR.Core.DTOs.LeaveRequestDTOs;
using HR.Core.Entities;
using HR.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly IMapper _mapper;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService, IMapper mapper)
    {
        _leaveRequestService = leaveRequestService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var leaveRequest = _mapper.Map<LeaveRequest>(dto);
        var createdRequest = await _leaveRequestService.CreateAsync(leaveRequest, employeeId);
        var leaveRequestDto = _mapper.Map<LeaveRequestResponseDto>(createdRequest);
        return CreatedAtAction(nameof(GetMyRequests), new { employeeId = leaveRequestDto.EmployeeId }, leaveRequestDto);
    }

    [HttpGet("my-requests")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyRequests()
    {
        var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var requests = await _leaveRequestService.GetMyRequestsAsync(employeeId);
        var requestDtos = _mapper.Map<List<LeaveRequestResponseDto>>(requests);
        return Ok(requestDtos);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPending()
    {
        var requests = await _leaveRequestService.GetPendingRequestsAsync();
        var requestDtos = _mapper.Map<List<LeaveRequestResponseDto>>(requests);
        return Ok(requestDtos);
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        await _leaveRequestService.UpdateStatusAsync(id, status);
        return NoContent();
    }
}