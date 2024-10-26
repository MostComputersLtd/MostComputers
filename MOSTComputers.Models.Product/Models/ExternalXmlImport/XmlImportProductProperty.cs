namespace MOSTComputers.Models.Product.Models.ExternalXmlImport;

public sealed class XmlImportProductProperty
{
    public int ProductId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Characteristic { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
    public string? XmlName { get; set; }
    public int? XmlDisplayOrder { get; set; }
}