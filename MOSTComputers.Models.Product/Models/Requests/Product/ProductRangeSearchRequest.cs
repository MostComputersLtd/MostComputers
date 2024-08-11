using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Requests.Product;

public sealed class ProductRangeSearchRequest
{
    public int Start { get; set; }
    public uint Length { get; set; }
}