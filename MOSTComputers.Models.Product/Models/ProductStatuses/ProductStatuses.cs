namespace MOSTComputers.Models.Product.Models.ProductStatuses;

public sealed class ProductStatuses
{
    public int ProductId { get; init; }
    public bool IsProcessed { get; init; }
    public bool NeedsToBeUpdated { get; init; }
}