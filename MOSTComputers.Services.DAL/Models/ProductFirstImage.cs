namespace MOSTComputers.Services.DAL.Models;

internal class ProductFirstImage
{
    public int Id { get; set; }
    public string? XML { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; set; }
}