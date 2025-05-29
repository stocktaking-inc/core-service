using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Models;

[Table("items")]
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("article")]
    public string Article { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("category")]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; } = 0;

    [ForeignKey("Location")]
    [Column("location")]
    public int? LocationId { get; set; }  // Made nullable to match your SQL data

    public Warehouse? Location { get; set; }

    [Column("status")]
    public StatusType Status { get; set; } = StatusType.OutOfStock;

    [ForeignKey("Supplier")]
    [Column("supplier")]
    public int SupplierId { get; set; }
    
    public Supplier? Supplier { get; set; }

    public enum StatusType
    {
        [Display(Name = "In Stock")]
        InStock,

        [Display(Name = "Out of Stock")]
        OutOfStock,

        [Display(Name = "Low Stock")]
        LowStock
    }
}