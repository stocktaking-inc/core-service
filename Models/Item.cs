using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreService.Models;

[Table("items")]
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [Column("article")]
    [MaxLength(50)]
    public string Article { get; set; }

    [Required]
    [Column("category")]
    [MaxLength(50)]
    public string Category { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("location")]
    public string Location { get; set; }

    [Required]
    [Column("status")]
    public ItemStatus Status { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemStatus
{
    Available,
    OutOfStock,
    Discontinued
}