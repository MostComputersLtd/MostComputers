namespace MOSTComputers.Models.Product.Models.ExternalXmlImport;

public sealed class ProductCharacteristicAndExternalXmlDataRelation
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public int? ProductCharacteristicId { get; init; }
    public required string XmlName { get; init; }
    public int XmlDisplayOrder { get; init; }
}