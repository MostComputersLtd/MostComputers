namespace MOSTComputers.Models.Product.Models.ProductStatuses;

public sealed class ProductStatuses
{
    public int ProductId { get; set; }
    public bool IsProcessed { get; set; }
    public bool NeedsToBeUpdated { get; set; }
}