using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("api/errors")]
//[Authorize(Roles = "Admin")]
public class ErrorController : ControllerBase
{
    private readonly IErrorRepository _errorRepository;
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(IErrorRepository errorRepository, ILogger<ErrorController> logger)
    {
        _errorRepository = errorRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ErrorResponseDto>>> GetAllErrors(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var errors = await _errorRepository.GetAllAsync(page, limit);
        return Ok(errors.Select(e => e.ToResponseDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ErrorResponseDto>> GetErrorDetails(int id)
    {
        var error = await _errorRepository.GetByIdAsync(id);
        if (error == null) return NotFound();
        return Ok(error.ToResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<ErrorResponseDto>> LogError([FromBody] ErrorDto errorDto)
    {
        var error = errorDto.ToModel();
        await _errorRepository.AddAsync(error);
        return CreatedAtAction(nameof(GetErrorDetails), new { id = error.Id }, error.ToResponseDto());
    }
}