using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Models;

[Table("order_items")]
public class OrderItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("order_id")]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Required]
    [Column("item_id")]
    public int ItemId { get; set; }

    [ForeignKey("ItemId")]
    public Item Item { get; set; }

    [Required]
    [Column("quantity")] 
    public int Quantity { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(10,2)")] 
    public decimal Price { get; set; }
    
    [NotMapped]
    public decimal TotalPrice => Quantity * Price;
}