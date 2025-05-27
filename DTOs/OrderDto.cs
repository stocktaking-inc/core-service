using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class OrderDto
    {
        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public List<OrderItemDto> Items { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public class OrderResponseDto : OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
    }

    public static class OrderDtoExtensions
    {
        public static Order ToModel(this OrderDto orderDto)
        {
            return new Order
            {
                OrderNumber = orderDto.OrderNumber,
                CustomerId = orderDto.CustomerId,
                Date = orderDto.Date,
                TotalAmount = orderDto.TotalAmount,
                Status = orderDto.Status,
                Items = orderDto.Items.Select(oi => new OrderItem
                {
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }

        public static OrderResponseDto ToResponseDto(this Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                Date = order.Date,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.Items.Select(oi => new OrderItemDto
                {
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}