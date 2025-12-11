namespace MOSTComputers.Models.Product.Models;

public sealed class ProductProperty
{
    public int ProductId { get; init; }
    public int? ProductCharacteristicId { get; init; }
    public int? DisplayOrder { get; init; }
    public string? Characteristic { get; init; }
    public string? Value { get; init; }
}