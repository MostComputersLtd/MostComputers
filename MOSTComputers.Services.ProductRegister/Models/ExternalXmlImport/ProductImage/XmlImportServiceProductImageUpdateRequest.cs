namespace MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;

public sealed class XmlImportServiceProductImageUpdateRequest
{
    public int Id { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
}