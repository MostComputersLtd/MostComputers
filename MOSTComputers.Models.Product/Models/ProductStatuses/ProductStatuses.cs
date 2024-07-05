using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ProductStatuses;

public sealed class ProductStatuses
{
    public int ProductId { get; set; }
    public bool IsProcessed { get; set; }
    public bool NeedsToBeUpdated { get; set; }
}