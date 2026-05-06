namespace AdmissionService.Application.DTOs;

public class AddProgramToAdmissionRequest
{
    public Guid ProgramId { get; set; }
    public int Priority { get; set; }
}
