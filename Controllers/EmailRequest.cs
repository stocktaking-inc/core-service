using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("api/emails")]
//[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<EmailRequestResponseDto>> SendEmail([FromBody] EmailRequestDto emailRequestDto)
    {
        var emailRequest = emailRequestDto.ToModel();
        await _emailService.SendEmailAsync(emailRequest);
        return Ok(emailRequest.ToResponseDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmailRequestResponseDto>> GetEmailStatus(int id)
    {
        var emailRequest = await _emailService.GetEmailRequestAsync(id);
        if (emailRequest == null) return NotFound();
        return Ok(emailRequest.ToResponseDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmailRequestResponseDto>>> GetAllEmails(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var emails = await _emailService.GetAllEmailRequestsAsync(page, limit);
        return Ok(emails.Select(e => e.ToResponseDto()));
    }
}