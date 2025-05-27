using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreService.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("order_number")]
    public required string OrderNumber { get; set; }

    [Required]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; }

    [Required]
    [Column("sum", TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column("status", TypeName = "varchar(20)")]
    public OrderStatus Status { get; set; }

    [Required]
    [Column("created_at", TypeName = "timestamp")]
    public DateTime Date { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Canceled
}