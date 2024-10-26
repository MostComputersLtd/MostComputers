using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;

public sealed class XmlImportProductPropertyByCharacteristicNameCreateRequest
{
    public int? ProductId { get; set; }
    public required string ProductCharacteristicName { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
    public string? XmlName { get; set; }
    public int? XmlDisplayOrder { get; set; }
}