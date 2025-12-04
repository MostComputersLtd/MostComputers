using MOSTComputers.Models.Product.Models.ProductIdentifiers;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductGTINCode;
public sealed class ProductGTINCodeCreateRequest
{
    public required int ProductId { get; init; }
    public required ProductGTINCodeType CodeType { get; init; }
    public required string CodeTypeAsText { get; init; }
    public required string Value { get; init; }

    public required string CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
}