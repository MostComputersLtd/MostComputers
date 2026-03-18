namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
public sealed class ServiceProductDocumentCreateRequest
{
    public required int ProductId { get; set; }
    public required byte[] FileData { get; set; }
    public required string FileExtension { get; set; }
    public string? Description { get; set; }
}