namespace AdmissionService.Application.DTOs;

public class CreateAdmissionRequest
{
    public List<AdmissionProgramDto> Programs { get; set; } = new();
}