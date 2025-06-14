namespace CoreService.DTOs
{
  public class ItemDTO
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int? LocationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SupplierId { get; set; }
  }
}
