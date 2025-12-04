using MOSTComputers.Models.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
public sealed class ServiceProductPropertyByCharacteristicNameCreateRequest
{
    public int ProductId { get; set; }
    public required string ProductCharacteristicName { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}