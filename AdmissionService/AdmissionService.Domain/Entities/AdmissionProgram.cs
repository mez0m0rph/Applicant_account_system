namespace AdmissionService.Domain.Entities;

public class AdmissionProgram
{
    public Guid Id { get; set; }
    public Guid AdmissionId { get; set; }
    public Guid ProgramId { get; set; }
    public int Priority { get; set; } 
}