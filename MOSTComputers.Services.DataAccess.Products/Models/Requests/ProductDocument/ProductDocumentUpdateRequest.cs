using OneOf;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductDocument;

public sealed class ProductDocumentUpdateRequest
{
    public required OneOf<int, string> IdOrFileName { get; set; }
    public string? Description { get; set; }
}