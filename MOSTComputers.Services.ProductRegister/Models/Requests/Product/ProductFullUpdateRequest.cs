using MOSTComputers.Models.Product.Models;
using MOSTComputers.Utils.ProductImageFileNameUtils;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.Product;

public sealed class ProductFullUpdateRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? AdditionalWarrantyPrice { get; set; }
    public long? AdditionalWarrantyTermMonths { get; set; }
    public string? StandardWarrantyPrice { get; set; }
    public long? StandardWarrantyTermMonths { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductStatusEnum? Status { get; set; }
    public int? PlShow { get; set; }
    public decimal? Price1 { get; set; }
    public decimal? DisplayPrice { get; set; }
    public decimal? Price3 { get; set; }
    public CurrencyEnum? Currency { get; set; }
    public Guid? RowGuid { get; set; }
    public int? PromotionId { get; set; }
    public int? PromRid { get; set; }
    public short? PromotionPictureId { get; set; }
    public DateTime? PromotionExpireDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpireDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? PartNumber1 { get; set; }
    public string? PartNumber2 { get; set; }
    public string? SearchString { get; set; }
    public int? CategoryId { get; set; }
    public MOSTComputers.Models.Product.Models.Category? Category { get; set; }
    public short? ManifacturerId { get; set; }
    public Manifacturer? Manifacturer { get; set; }
    public int? SubCategoryId { get; set; }
    public List<ProductProperty>? Properties { get; set; }
    public List<ImageAndImageFileNameRelation>? ImagesAndFileNames { get; set; }
}