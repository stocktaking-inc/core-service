using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Models;

[Table("good")]
public class Good
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [StringLength(50)]
  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [Required]
  [StringLength(20)]
  [Column("article")]
  public string Article { get; set; } = string.Empty;

  [Column("purchase_price", TypeName = "decimal(10, 2)")]
  public decimal? PurchasePrice { get; set; }

  [StringLength(50)]
  [Column("category")]
  public string? Category { get; set; }

  [Column("supplier_id")]
  public int SupplierId { get; set; }

  [ForeignKey("SupplierId")]
  public Supplier Supplier { get; set; } = null!; // Убираем nullable
}
