namespace MOSTComputers.Models.Product.Models;

public sealed class ProductCharacteristic
{
    public int Id { get; init; }
    public int? CategoryId { get; init; }
    public string? Name { get; init; }
    public string? Meaning { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? Active { get; init; }
    public short? PKUserId { get; init; }
    public DateTime? LastUpdate { get; init; }
    public ProductCharacteristicType? KWPrCh { get; init; }
}