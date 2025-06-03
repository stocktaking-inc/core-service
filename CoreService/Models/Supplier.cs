using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CoreService.Models;

[Table("suppliers")]
public class Supplier
{
  [Key]
  [Column("supplier_id")]
  public int Id { get; set; }

  [Required]
  [StringLength(100)]
  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [StringLength(100)]
  [Column("contact_person")]
  public string? ContactPerson { get; set; }

  [StringLength(100)]
  [Column("email")]
  public string? Email { get; set; }

  [StringLength(20)]
  [Column("phone")]
  public string? Phone { get; set; }

  [Column("status")]
  public EntityStatus Status { get; set; } = EntityStatus.Active;

  public enum EntityStatus
  {
    [PgName("active")]
    Active,
    [PgName("inactive")]
    Inactive
  }
}
