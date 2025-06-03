using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

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
  public int Quantity { get; set; }

  [ForeignKey("Location")]
  [Column("location")]
  public int? LocationId { get; set; }

  public Warehouse? Location { get; set; }

  [Column("supplier")]
  public int SupplierId { get; set; }

  [ForeignKey("SupplierId")]
  public Supplier? Supplier { get; set; }
}

// public enum ItemStatusType
// {
//   [PgName("Out of Stock")]
//   OutOfStock = 0,
//
//   [PgName("In Stock")]
//   InStock = 1,
//
//   [PgName("Low Stock")]
//   LowStock = 2
// }
