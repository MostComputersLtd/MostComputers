namespace MOSTComputers.UI.Web.Models.PromotionFiles.DTOs;

public sealed class PromotionFileUpdateDTO
{
    public int? Id { get; init; }
    public string? Name { get; init; }
    public bool Active { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public IFormFile? File { get; init; }
    public string? Description { get; init; }
    public string? RelatedProductsString { get; init; }
}