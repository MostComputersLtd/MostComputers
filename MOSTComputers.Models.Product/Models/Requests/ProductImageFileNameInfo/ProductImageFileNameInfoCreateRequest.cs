namespace MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoCreateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
}