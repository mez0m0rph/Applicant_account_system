namespace ProgramService.Application.DTOs;

public class PagedProgramsResponse
{
    public List<ProgramDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
