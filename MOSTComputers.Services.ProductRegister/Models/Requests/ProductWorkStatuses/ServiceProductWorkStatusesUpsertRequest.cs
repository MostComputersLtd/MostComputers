using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
public sealed class ServiceProductWorkStatusesUpsertRequest
{
    public required int ProductId { get; set; }
    public required ProductNewStatus ProductNewStatus { get; set; }
    public required ProductXmlStatus ProductXmlStatus { get; set; }
    public required bool ReadyForImageInsert { get; set; }
    public required string UpsertUserName { get; init; }
}