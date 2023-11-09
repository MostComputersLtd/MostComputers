namespace MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;

public sealed class ProductCharacteristicByIdUpdateRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Meaning { get; set; }
    public int? DisplayOrder { get; set; }
    public short? Active { get; set; }
    public short? PKUserId { get; set; }
    public DateTime? LastUpdate { get; set; }
    public short? KWPrCh { get; set; }
}