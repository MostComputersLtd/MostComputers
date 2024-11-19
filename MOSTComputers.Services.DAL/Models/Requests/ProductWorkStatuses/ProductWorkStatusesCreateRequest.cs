using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;
public sealed class ProductWorkStatusesCreateRequest
{
    public int ProductId { get; set; }
    public ProductNewStatusEnum ProductNewStatus { get; set; }
    public ProductXmlStatusEnum ProductXmlStatus { get; set; }
    public bool ReadyForImageInsert { get; set; }
}