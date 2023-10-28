using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductProperty;

public class ProductPropertyUpdateRequest
{
    public int ProductId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}