namespace MOSTComputers.Models.Product.Models;

public sealed class Product
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
    public decimal? Price { get; set; }
    public CurrencyEnum? Currency { get; set; }
    public Guid? RowGuid { get; set; }
    public int? Promotionid { get; set; }
    public int? PromRid { get; set; }
    public short? PromotionPictureId { get; set; }
    public DateTime? PromotionExpireDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpireDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? PartNumber1 { get; set; }
    public string? PartNumber2 { get; set; }
    public string? SearchString { get; set; }
    public List<ProductProperty> Properties { get; set; }
    public List<ProductImage>? Images { get; set; }
    public List<ProductImageFileNameInfo>? ImageFileNames { get; set; }
    public int? CategoryID { get; set; }
    public Category? Category { get; set; }
    public short? ManifacturerId { get; set; }
    public Manifacturer? Manifacturer { get; set; }
    public int? SubCategoryId { get; set; }
}