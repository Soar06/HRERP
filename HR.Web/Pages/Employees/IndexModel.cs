using AutoMapper;
using HR.Core.DTOs.EmployeeDTOs;
using HR.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HR.Web.Pages.Employees;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;

    public IndexModel(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
    }

    public List<EmployeeResponseDto> Employees { get; set; } = new();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("HRAPI");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.User.FindFirst("token")?.Value);
        var response = await client.GetFromJsonAsync<List<Employee>>("api/employees?page=1&pageSize=10");
        Employees = _mapper.Map<List<EmployeeResponseDto>>(response ?? new List<Employee>());
    }
}