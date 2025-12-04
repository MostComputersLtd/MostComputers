using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
public sealed class ProductWorkStatusesUpdateByProductIdRequest
{
    public required int ProductId { get; set; }
    public required ProductNewStatus ProductNewStatus { get; set; }
    public required ProductXmlStatus ProductXmlStatus { get; set; }
    public required bool ReadyForImageInsert { get; set; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}