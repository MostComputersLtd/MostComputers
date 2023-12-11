using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Requests.Product;

public sealed class ProductConditionalSearchRequest
{
    public ProductStatusEnum? Status { get; set; }
    public string? NameSubstring { get; set; }
    public string? SearchStringSubstring { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsProcessed { get; set; }
    public bool? NeedsToBeUpdated { get; set; }
}