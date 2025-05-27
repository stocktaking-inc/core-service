using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class ItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Article { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public ItemStatus Status { get; set; }
    }

    public class ItemResponseDto : ItemDto
    {
        public int Id { get; set; }
    }

    public static class ItemDtoExtensions
    {
        public static ItemDto ToDto(this Item item)
        {
            return new ItemDto
            {
                Name = item.Name,
                Article = item.Article,
                Category = item.Category,
                Quantity = item.Quantity,
                Location = item.Location,
                Status = item.Status
            };
        }

        public static Item ToModel(this ItemDto itemDto)
        {
            return new Item
            {
                Name = itemDto.Name,
                Article = itemDto.Article,
                Category = itemDto.Category,
                Quantity = itemDto.Quantity,
                Location = itemDto.Location,
                Status = itemDto.Status
            };
        }

        public static ItemResponseDto ToResponseDto(this Item item)
        {
            return new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Article = item.Article,
                Category = item.Category,
                Quantity = item.Quantity,
                Location = item.Location,
                Status = item.Status
            };
        }
    }
}