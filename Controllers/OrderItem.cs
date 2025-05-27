using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreService.Models;
using CoreService.Interfaces;
using CoreService.DTOs;

[ApiController]
[Route("api/orders/{orderId}/items")]
//[Authorize]
public class OrderItemController : ControllerBase
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<OrderItemController> _logger;

    public OrderItemController(
        IOrderItemRepository orderItemRepository,
        IOrderRepository orderRepository,
        IItemRepository itemRepository,
        ILogger<OrderItemController> logger)
    {
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetOrderItems(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "ORDER_NOT_FOUND",
                    Message = "Order not found",
                    Timestamp = DateTime.UtcNow
            });
        }

        var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
        return Ok(orderItems.Select(oi => oi.ToResponseDto()));
    }

    [HttpPost]
    public async Task<ActionResult<OrderItemResponseDto>> AddOrderItem(int orderId, [FromBody] OrderItemDto orderItemDto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "ORDER_NOT_FOUND",
                    Message = "Order not found",
                    Timestamp = DateTime.UtcNow
            });
        }

        var item = await _itemRepository.GetByIdAsync(orderItemDto.ItemId);
        if (item == null)
        {
            return BadRequest(new ErrorResponseDto {
                    Id = 0,
                    Code = "ITEM_NOT_FOUND",
                    Message = "Item not found",
                    Timestamp = DateTime.UtcNow
            });
        }

        var orderItem = orderItemDto.ToModel(orderId);
        await _orderItemRepository.AddAsync(orderItem);

        order.TotalAmount = (await _orderItemRepository.GetByOrderIdAsync(orderId))
            .Sum(oi => oi.TotalPrice);
        await _orderRepository.UpdateAsync(order);

        return CreatedAtAction(
            nameof(GetOrderItem),
            new { orderId = orderId, id = orderItem.Id },
            orderItem.ToResponseDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItemResponseDto>> GetOrderItem(int orderId, int id)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem == null || orderItem.OrderId != orderId)
        {
            return NotFound();
        }
        return Ok(orderItem.ToResponseDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderItemResponseDto>> UpdateOrderItem(int orderId, int id, [FromBody] OrderItemDto orderItemDto)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem == null || orderItem.OrderId != orderId)
        {
            return NotFound();
        }

        var originalTotal = orderItem.TotalPrice;
        
        orderItem.ItemId = orderItemDto.ItemId;
        orderItem.Quantity = orderItemDto.Quantity;
        orderItem.Price = orderItemDto.Price;

        await _orderItemRepository.UpdateAsync(orderItem);

        if (originalTotal != orderItem.TotalPrice)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.TotalAmount += (orderItem.TotalPrice - originalTotal);
                await _orderRepository.UpdateAsync(order);
            }
        }

        return Ok(orderItem.ToResponseDto());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveOrderItem(int orderId, int id)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem == null || orderItem.OrderId != orderId)
        {
            return NotFound();
        }

        await _orderItemRepository.DeleteAsync(orderItem);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null)
        {
            order.TotalAmount -= orderItem.TotalPrice;
            await _orderRepository.UpdateAsync(order);
        }

        return NoContent();
    }
}