namespace AdmissionService.Application.DTOs;

public class GetAdmissionsQuery
{
    public string? Search { get; set; }
    public Guid? ProgramId { get; set; }
    public List<string> Faculties { get; set; } = new();
    public string? Status { get; set; }
    public bool OnlyUnassigned { get; set; }
    public bool OnlyMine { get; set; }
    public Guid? AssignedManagerUserId { get; set; }

    public string SortBy { get; set; } = "updatedAt";
    public string SortDirection { get; set; } = "desc";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
