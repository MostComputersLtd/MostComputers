using OneOf;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;

public sealed class ServiceProductDocumentUpdateRequest
{
    public OneOf<int, string> IdOrFileName { get; set; }
    public string? Description { get; set; }
}