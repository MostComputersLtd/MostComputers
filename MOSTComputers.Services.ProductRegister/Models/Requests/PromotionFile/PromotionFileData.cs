namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;

public sealed class PromotionFileData
{
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required byte[] Data { get; set; }
}
