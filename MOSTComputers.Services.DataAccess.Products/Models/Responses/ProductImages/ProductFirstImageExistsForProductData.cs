namespace MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
public sealed class ProductFirstImageExistsForProductData
{
    public int ProductId { get; init; }
    public bool FirstImageExists { get; init; }
}