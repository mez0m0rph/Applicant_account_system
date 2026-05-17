namespace AdmissionService.Application.DTOs;

public class PagedAdmissionsResponse
{
    public List<AdmissionResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
