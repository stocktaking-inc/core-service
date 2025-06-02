using CoreService.Models;

namespace CoreService.DTOs;

public class SupplierDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? ContactPerson { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public Supplier.EntityStatus Status { get; set; } = Supplier.EntityStatus.Active;
}