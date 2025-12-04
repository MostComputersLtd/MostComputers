namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;

public sealed class ImageFileData
{
    public required string FileName { get; init; }
    public byte[]? ImageData { get; init; }
}