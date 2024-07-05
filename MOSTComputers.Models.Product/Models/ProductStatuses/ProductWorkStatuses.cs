namespace MOSTComputers.Models.Product.Models.ProductStatuses;

public sealed class ProductWorkStatuses
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductNewStatusEnum ProductNewStatus { get; set; }
    public ProductXmlStatusEnum ProductXmlStatus { get; set; }
    public bool ReadyForImageInsert { get; set; }
}