namespace CoreService.DTOs;

public class GoodDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Article { get; set; } = string.Empty;
  public decimal? PurchasePrice { get; set; }
  public string? Category { get; set; }
  public int SupplierId { get; set; }
}
