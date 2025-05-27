using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class OrderItemDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }

    public class OrderItemResponseDto : OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public static class OrderItemDtoExtensions
    {
        public static OrderItem ToModel(this OrderItemDto orderItemDto, int orderId = 0)
        {
            return new OrderItem
            {
                ItemId = orderItemDto.ItemId,
                Quantity = orderItemDto.Quantity,
                Price = orderItemDto.Price,
                OrderId = orderId
            };
        }

        public static OrderItemResponseDto ToResponseDto(this OrderItem orderItem)
        {
            return new OrderItemResponseDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ItemId = orderItem.ItemId,
                ItemName = orderItem.Item?.Name,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price,
                TotalPrice = orderItem.Quantity * orderItem.Price
            };
        }
    }
}