using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("orders")]
//[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IItemRepository itemRepository,
        ILogger<OrderController> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var orders = await _orderRepository.GetAllAsync(page, limit);
        return Ok(orders.Select(o => o.ToResponseDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderDetails(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (order == null) return NotFound();
        return Ok(order.ToResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> AddOrder([FromBody] OrderDto orderDto)
    {
        var customer = await _customerRepository.GetByIdAsync(orderDto.CustomerId);
        if (customer == null) return BadRequest();

        foreach (var item in orderDto.Items)
        {
            var dbItem = await _itemRepository.GetByIdAsync(item.ItemId);
            if (dbItem == null || dbItem.Status != ItemStatus.Available) return BadRequest();
        }

        var order = orderDto.ToModel();
        order.TotalAmount = orderDto.Items.Sum(i => i.Quantity * i.Price);

        await _orderRepository.AddAsync(order);
        return CreatedAtAction(nameof(GetOrderDetails), new { id = order.Id }, order.ToResponseDto());
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return NotFound();
        order.Status = statusDto.Status;
        await _orderRepository.UpdateAsync(order);
        return Ok(order.ToResponseDto());
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderResponseDto>> CancelOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status == OrderStatus.Canceled) return BadRequest();
        order.Status = OrderStatus.Canceled;
        await _orderRepository.UpdateAsync(order);
        return Ok(order.ToResponseDto());
    }

    [HttpPost("{id}/invoice")]
    public async Task<ActionResult<Invoice>> GenerateInvoice(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (order == null) return NotFound();
        var invoice = new Invoice
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            IssuedDate = DateTime.UtcNow
        };
        return CreatedAtAction(nameof(GetOrderDetails), new { id = order.Id }, invoice);
    }
}