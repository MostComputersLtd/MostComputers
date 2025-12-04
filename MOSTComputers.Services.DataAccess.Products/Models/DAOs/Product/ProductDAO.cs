using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.Models.DAOs.Product;
internal sealed class ProductDAO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? AdditionalWarrantyPrice { get; set; }
    public long? AdditionalWarrantyTermMonths { get; set; }
    public string? StandardWarrantyPrice { get; set; }
    public long? StandardWarrantyTermMonths { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductStatus? Status { get; set; }
    public int? PlShow { get; set; }
    public decimal? Price { get; set; }
    public Currency? Currency { get; set; }
    public Guid? RowGuid { get; set; }
    public int? PromotionPid { get; set; }
    public int? PromotionRid { get; set; }
    public short? PromotionPictureId { get; set; }
    public DateTime? PromotionExpireDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpireDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? PartNumber1 { get; set; }
    public string? PartNumber2 { get; set; }
    public string? SearchString { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public short? ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public int? SubCategoryId { get; set; }
}