using MOSTComputers.Models.Product.Models.ProductStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
public sealed class ProductWorkStatusesCreateRequest
{
    public int ProductId { get; set; }
    public ProductNewStatusEnum ProductNewStatus { get; set; }
    public ProductXmlStatusEnum ProductXmlStatus { get; set; }
    public bool ReadyForImageInsert { get; set; }
}