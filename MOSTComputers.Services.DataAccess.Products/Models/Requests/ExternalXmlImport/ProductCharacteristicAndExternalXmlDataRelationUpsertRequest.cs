namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;

public sealed class ProductCharacteristicAndExternalXmlDataRelationUpsertRequest
{
    public required int CategoryId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public required string XmlName { get; set; }
    public int? XmlDisplayOrder { get; set; }
}