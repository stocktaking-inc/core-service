using System.ComponentModel.DataAnnotations;
using CoreService.Models;
namespace CoreService.DTOs
{
    public class CustomerDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public CustomerStatus Status { get; set; }
    }

    public class CustomerResponseDto : CustomerDto
    {
        public int Id { get; set; }
        public decimal TotalPurchases { get; set; }
        public DateTime? LastOrder { get; set; }
        public List<string> Orders { get; set; } = new List<string>();
    }

    public static class CustomerDtoExtensions
    {
        public static Customer ToModel(this CustomerDto customerDto)
        {
            return new Customer
            {
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone,
                Status = customerDto.Status
            };
        }

        public static CustomerResponseDto ToResponseDto(this Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Status = customer.Status,
                TotalPurchases = customer.TotalPurchases,
                LastOrder = customer.LastOrder,
                Orders = customer.Orders.Select(o => o.OrderNumber).ToList()
            };
        }
    }
}