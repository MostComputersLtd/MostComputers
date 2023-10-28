namespace MOSTComputers.Services.DAL.Models;

public class Product
{
    public int Id { get; set; }
    public string? SubType { get; set; }
    public decimal? AddWrr { get; set; }
    public long? AddWrrTerm { get; set; }
    public string? AddWrrDef { get; set; }
    public long? DefWrrTerm { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductStatusEnum? Status { get; set; }
    public int? PlShow { get; set; }
    public decimal? Price1 { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }
    public CurrencyEnum? Currency { get; set; }
    public Guid? RowGuid { get; set; }
    public int? PromPid { get; set; }
    public int? PromRid { get; set; }
    public short? PromPictureId { get; set; }
    public DateTime? PromExpDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? SplModel1 { get; set; }
    public string? SplModel2 { get; set; }
    public string? SplModel3 { get; set; }
    public List<ProductProperty> Properties { get; set; }
    public int? CategoryID { get; set; }
    public short? ManifacturerId { get; set; }
    public int? SubCategoryId { get; set; }
}
