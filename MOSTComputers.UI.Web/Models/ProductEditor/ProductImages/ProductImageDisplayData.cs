namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;

public sealed class ProductImageDisplayData
{
    public required int? ProductId { get; init; }
    public required int? ExistingImageId { get; init; }
    public string? ContentType { get; init; }
    public string? HtmlData { get; init; }
    public DateTime? DateModified { get; init; }
}