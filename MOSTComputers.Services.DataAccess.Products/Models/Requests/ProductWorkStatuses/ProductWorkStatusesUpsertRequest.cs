using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
public sealed class ProductWorkStatusesUpsertRequest
{
    public int ProductId { get; set; }
    public ProductNewStatus ProductNewStatus { get; set; }
    public ProductXmlStatus ProductXmlStatus { get; set; }
    public bool ReadyForImageInsert { get; set; }
    public required string UpsertUserName { get; init; }
    public required DateTime UpsertDate { get; init; }
}