using System.ComponentModel.DataAnnotations;
using CoreService.Models;

namespace CoreService.DTOs
{
    public class InvoiceDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    public class InvoiceResponseDto : InvoiceDto
    {
        public int Id { get; set; }
        public DateTime IssuedDate { get; set; }
        public string InvoiceNumber { get; set; } = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    public static class InvoiceDtoExtensions
    {
        public static Invoice ToModel(this InvoiceDto invoiceDto)
        {
            return new Invoice
            {
                OrderId = invoiceDto.OrderId,
                Amount = invoiceDto.Amount
            };
        }

        public static InvoiceResponseDto ToResponseDto(this Invoice invoice)
        {
            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderId = invoice.OrderId,
                Amount = invoice.Amount,
                IssuedDate = invoice.IssuedDate,
                InvoiceNumber = $"INV-{invoice.Id.ToString().PadLeft(6, '0')}"
            };
        }
    }
}