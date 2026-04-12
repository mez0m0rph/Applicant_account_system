using ApplicantService.Application.DTOs;
using ApplicantService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantService.API.Controllers;

[ApiController]
[Route("applicant")]
[Authorize]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _applicantService;

    public ApplicantController(IApplicantService applicantService)
    {
        _applicantService = applicantService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        try
        {
            await _applicantService.CreateAsync(request);
            return Ok("Профиль создан");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}