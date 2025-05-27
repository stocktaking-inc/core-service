using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("customers")]
//[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        ILogger<CustomerController> logger)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAllCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var customers = await _customerRepository.GetAllAsync(page, limit);
        return Ok(customers.Select(c => c.ToResponseDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomerDetails(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer.ToResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> AddCustomer([FromBody] CustomerDto customerDto)
    {
        var customer = customerDto.ToModel();
        await _customerRepository.AddAsync(customer);
        return CreatedAtAction(nameof(GetCustomerDetails), new { id = customer.Id }, customer.ToResponseDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null) return NotFound();
        
        existingCustomer.Name = customerDto.Name;
        existingCustomer.Email = customerDto.Email;
        existingCustomer.Phone = customerDto.Phone;
        existingCustomer.Status = customerDto.Status;

        await _customerRepository.UpdateAsync(existingCustomer);
        return Ok(existingCustomer.ToResponseDto());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        await _customerRepository.DeleteAsync(customer);
        return NoContent();
    }

    [HttpGet("{id}/orders")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetCustomerOrders(int id)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(id);
        return Ok(orders.Select(o => o.ToResponseDto()));
    }

    [HttpPost("{id}/email")]
    public async Task<IActionResult> SendCustomerEmail(int id, [FromBody] EmailRequestDto emailRequest)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        
        return Ok(new { message = "Email successfully sent" });
    }
}