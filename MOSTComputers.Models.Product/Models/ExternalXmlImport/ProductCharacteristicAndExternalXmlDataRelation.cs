using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ExternalXmlImport;

public sealed class ProductCharacteristicAndExternalXmlDataRelation
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public string? ProductCharacteristicName { get; set; }
    public string? ProductCharacteristicMeaning { get; set; }
    public required string XmlName { get; set; }
    public int XmlDisplayOrder { get; set; }
}