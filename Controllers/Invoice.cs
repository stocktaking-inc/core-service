using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("api/invoices")]
//[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        ILogger<InvoiceController> logger)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> CreateInvoice([FromBody] InvoiceDto invoiceDto)
    {
        var order = await _orderRepository.GetByIdAsync(invoiceDto.OrderId);
        if (order == null)
        {
            return BadRequest(new ErrorResponseDto {
                Id = 0,
                Code = "ORDER_NOT_FOUND",
                Message = "Order not found",
                Timestamp = DateTime.UtcNow
            });
        }

        var invoice = invoiceDto.ToModel();
        invoice.IssuedDate = DateTime.UtcNow;
        
        await _invoiceRepository.AddAsync(invoice);
        return CreatedAtAction(nameof(GetInvoiceDetails), new { id = invoice.Id }, invoice.ToResponseDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetInvoiceDetails(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }
        return Ok(invoice.ToResponseDto());
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetInvoicesForOrder(int orderId)
    {
        var invoices = await _invoiceRepository.GetByOrderIdAsync(orderId);
        return Ok(invoices.Select(i => i.ToResponseDto()));
    }

    [HttpPost("{id}/send")]
    public async Task<IActionResult> SendInvoice(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        return Ok(new { message = "Invoice sent successfully" });
    }
}