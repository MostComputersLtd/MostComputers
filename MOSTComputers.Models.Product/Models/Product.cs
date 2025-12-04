namespace MOSTComputers.Models.Product.Models;

public sealed class Product
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public decimal? AdditionalWarrantyPrice { get; init; }
    public long? AdditionalWarrantyTermMonths { get; init; }
    public string? StandardWarrantyPrice { get; init; }
    public long? StandardWarrantyTermMonths { get; init; }
    public int? DisplayOrder { get; init; }
    public ProductStatus? Status { get; init; }
    public int? PlShow { get; init; }
    public decimal? Price { get; init; }
    public Currency? Currency { get; init; }
    public Guid? RowGuid { get; init; }
    public int? PromotionPid { get; init; }
    public int? PromotionRid { get; init; }
    public short? PromotionPictureId { get; init; }
    public DateTime? PromotionExpireDate { get; init; }
    public short? AlertPictureId { get; init; }
    public DateTime? AlertExpireDate { get; init; }
    public string? PriceListDescription { get; init; }
    public string? PartNumber1 { get; init; }
    public string? PartNumber2 { get; init; }
    public string? SearchString { get; init; }
    public int? CategoryId { get; init; }
    public Category? Category { get; init; }
    public short? ManufacturerId { get; init; }
    public Manufacturer? Manufacturer { get; init; }
    public int? SubCategoryId { get; init; }
}