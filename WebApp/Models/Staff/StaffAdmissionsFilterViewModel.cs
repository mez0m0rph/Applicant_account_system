namespace WebApp.Models.Staff;

public class StaffAdmissionsFilterViewModel
{
    public string? Search { get; set; }
    public Guid? ProgramId { get; set; }
    public string? Faculty { get; set; }
    public string? Status { get; set; }
    public bool OnlyUnassigned { get; set; }
    public Guid? AssignedManagerUserId { get; set; }

    public string SortBy { get; set; } = "updatedAt";
    public string SortDirection { get; set; } = "desc";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
