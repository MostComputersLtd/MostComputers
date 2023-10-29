namespace MOSTComputers.Services.DAL.Models;

public sealed class ProductProperty
{
    public int ProductId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Characteristic { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}