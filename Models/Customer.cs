using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreService.Models;

[Table("customers")]
public class Customer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [Column("phone")]
    public string Phone { get; set; }

    [Column("total_purchases")]
    public decimal TotalPurchases { get; set; }

    [Column("last_order")]
    public DateTime? LastOrder { get; set; }

    [Column("status")]
    public CustomerStatus Status { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerStatus
{
    Active,
    Inactive
}