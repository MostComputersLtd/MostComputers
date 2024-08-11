using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;

public sealed class FailedPropertyNameOfProductMultiCreateRequest
{
    public int ProductId { get; set; }
    public required HashSet<string> PropertyNames { get; init; }
}