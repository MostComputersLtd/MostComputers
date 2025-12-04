using MOSTComputers.Models.Product.Models.ProductIdentifiers;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;

public sealed class ServiceProductGTINCodeUpdateRequest
{
    public required int ProductId { get; init; }
    public required ProductGTINCodeType CodeType { get; init; }
    public required string CodeTypeAsText { get; init; }
    public required string Value { get; init; }
    public required string UpdateUserName { get; init; }
}