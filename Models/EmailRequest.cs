using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreService.Models;

[Table("email_requests")]
public class EmailRequest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("subject")]
    public string Subject { get; set; }

    [Required]
    [Column("body")]
    public string Body { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("status")]
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailStatus
{
    Pending,
    Sent,
    Failed
}