namespace MOSTComputers.Services.DAL.Models.Requests.ProductStatuses;

public sealed class ProductStatusesCreateRequest
{
    public int ProductId { get; set; }
    public bool IsProcessed { get; set; }
    public bool NeedsToBeUpdated { get; set; }
}