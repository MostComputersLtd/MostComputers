namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;

public sealed class ProductPropertyUpsertAllForProductRequest
{
    public required int ProductId { get; set; }
    public required List<ProductPropertyForProductUpsertRequest> NewProperties { get; set; }
}
