namespace MOSTComputers.Services.ProductRegister.Models.Responses;
public sealed class GroupPromotionImageFile
{
    public required int Id { get; init; }
    public int? PromotionId { get; init; }
    public required int ImageId { get; init; }
    public required string FileName { get; init; }
    public Stream? FileDataStream { get; init; }
}