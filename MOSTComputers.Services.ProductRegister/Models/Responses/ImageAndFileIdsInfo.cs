namespace MOSTComputers.Services.ProductRegister.Models.Responses;

public sealed class ImageAndFileIdsInfo
{
    public required int ImageId { get; init; }
    public int? FileInfoId { get; init; }
}