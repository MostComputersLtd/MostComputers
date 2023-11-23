using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Requests.ProductProperty;

public sealed class ProductPropertyByCharacteristicNameCreateRequest
{
    public int ProductId { get; set; }
    public string ProductCharacteristicName { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}