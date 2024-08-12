namespace MOSTComputers.Models.Product.Models.Requests.Product;
public sealed class ProductUpdateRequest
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
    public List<CurrentProductPropertyUpdateRequest> Properties { get; set; }
    public List<CurrentProductImageUpdateRequest>? Images { get; set; }
    public List<CurrentProductImageFileNameInfoUpdateRequest>? ImageFileNames { get; set; }
    public int? CategoryId { get; set; }
    public short? ManifacturerId { get; set; }
    public int? SubCategoryId { get; set; }
}

public sealed class CurrentProductPropertyUpdateRequest
{
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}

public sealed class CurrentProductImageUpdateRequest
{
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime? DateModified { get; set; }
}

public sealed class CurrentProductImageFileNameInfoUpdateRequest
{
    public string? FileName { get; set; }
    public int ImageNumber { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool Active { get; set; }
}