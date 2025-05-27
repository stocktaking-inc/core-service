using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Models;

[Table("errors")]
public class Error
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    public string Code { get; set; }

    [Required]
    [Column("message")]
    public string Message { get; set; }
}