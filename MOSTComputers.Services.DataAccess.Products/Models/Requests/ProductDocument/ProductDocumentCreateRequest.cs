namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductDocument;
public sealed class ProductDocumentCreateRequest
{
    public required int ProductId { get; set; }
    public required string FileExtension { get; set; }
    public string? Description { get; set; }
}