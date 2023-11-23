using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;

public sealed class FailedPropertyNameOfProductUpdateRequest
{
    public uint ProductId { get; set; }
    public string OldPropertyName { get; set; }
    public string NewPropertyName { get; set; }
}