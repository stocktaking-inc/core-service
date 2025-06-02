using CoreService.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CoreService.DTOs;

public class SupplierDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? ContactPerson { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }

  [JsonConverter(typeof(EntityStatusConverter))]
  public Supplier.EntityStatus Status { get; set; } = Supplier.EntityStatus.Active;
}

public class EntityStatusConverter : JsonConverter<Supplier.EntityStatus>
{
  public override Supplier.EntityStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var value = reader.GetString();
    if (string.IsNullOrEmpty(value))
      return Supplier.EntityStatus.Active;

    return value.ToLower() switch
    {
      "active" => Supplier.EntityStatus.Active,
      "inactive" => Supplier.EntityStatus.Inactive,
      _ => Supplier.EntityStatus.Active
    };
  }

  public override void Write(Utf8JsonWriter writer, Supplier.EntityStatus value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToString());
  }
}
