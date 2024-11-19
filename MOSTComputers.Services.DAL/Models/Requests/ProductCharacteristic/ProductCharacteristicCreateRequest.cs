using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

public sealed class ProductCharacteristicCreateRequest
{
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Meaning { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
    public short? PKUserId { get; set; }
    public DateTime? LastUpdate { get; } = null;
    public ProductCharacteristicTypeEnum? KWPrCh { get; set; }
}