using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Models;

[Table("warehouse")]
public class Warehouse
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [StringLength(100)]
  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [Column("address")]
  public string? Address { get; set; }

  [Column("is_active")]
  public bool IsActive { get; set; } = true;
}