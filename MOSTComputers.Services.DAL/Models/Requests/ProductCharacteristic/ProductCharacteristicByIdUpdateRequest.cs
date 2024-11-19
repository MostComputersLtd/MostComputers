using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

public sealed class ProductCharacteristicByIdUpdateRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Meaning { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
    public short? PKUserId { get; set; }
    public DateTime? LastUpdate { get; set; }
    public ProductCharacteristicTypeEnum? KWPrCh { get; set; }
}