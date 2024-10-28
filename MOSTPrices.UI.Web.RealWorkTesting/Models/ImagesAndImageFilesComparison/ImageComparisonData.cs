using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.ImagesAndImageFilesComparison;

public class ImageComparisonData
{
    public required OneOf<int, string> IdOrFileName { get; init; }
    public required int ProductId { get; init; }
    public required string? ImageContentType { get; init; }
    public byte[]? ImageData { get; init; }
}
