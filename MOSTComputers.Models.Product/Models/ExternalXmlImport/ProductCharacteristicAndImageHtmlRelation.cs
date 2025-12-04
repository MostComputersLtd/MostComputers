using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ExternalXmlImport;
public sealed class ProductCharacteristicAndImageHtmlRelation
{
    public required int Id { get; init; }
    public required int CategoryId { get; init; }
    public int? ProductCharacteristicId { get; init; }
    public required string HtmlName { get; init; }
}