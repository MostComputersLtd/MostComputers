using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
public sealed class ServiceProductPropertyByCharacteristicIdCreateRequest
{
    public int ProductId { get; set; }
    public int ProductCharacteristicId { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
}