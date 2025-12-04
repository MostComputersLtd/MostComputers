namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;

public sealed class ProductImageFileCreateRequest
{
    public required int ProductId { get; set; }
    public int? ImageId { get; set; }
    public FileData? FileData { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public bool? Active { get; set; }
    public required string CreateUserName { get; init; }
}