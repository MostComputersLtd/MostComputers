using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;

public sealed class XmlImportProductPropertyByCharacteristicIdCreateRequest
{
    public int? ProductId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
    public string? XmlName { get; set; }
    public int? XmlDisplayOrder { get; set; }
}