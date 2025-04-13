namespace HR.Core.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int RequiredMembers { get; set; }
    public int RequiredFreshers { get; set; }
    public int RequiredJuniors { get; set; }
    public int RequiredMidLevels { get; set; }
    public int RequiredSeniors { get; set; }
    public List<EmployeeProject> EmployeeProjects { get; set; } = new();
}