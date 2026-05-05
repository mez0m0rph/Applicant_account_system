namespace ProgramService.Application.DTOs.External;

public class ExternalProgramsResponse
{
    public List<ExternalProgramDto> Programs { get; set; } = new();
}

public class ExternalProgramDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;

    public ExternalFacultyDto Faculty { get; set; } = new();
    public ExternalEducationLevelDto EducationLevel { get; set; } = new();
}

public class ExternalFacultyDto
{
    public string Name { get; set; } = string.Empty;
}

public class ExternalEducationLevelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
