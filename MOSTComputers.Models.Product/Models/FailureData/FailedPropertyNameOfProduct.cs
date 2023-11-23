using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.FailureData;

public sealed class FailedPropertyNameOfProduct
{
    public int ProductId { get; set; }
    public string PropertyName { get; set; }
}