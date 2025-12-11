using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
public sealed class ProductPropertyUpdateRequest
{
    public int ProductId { get; set; }
    public int ProductCharacteristicId { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
}