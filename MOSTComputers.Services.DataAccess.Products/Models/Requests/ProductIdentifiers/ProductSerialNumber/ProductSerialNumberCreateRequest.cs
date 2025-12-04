namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;

public sealed class ProductSerialNumberCreateRequest
{
    public required int ProductId { get; init; }
    public required string SerialNumber { get; init; }
}