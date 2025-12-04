namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;

public sealed class ProductImageFileRenameRequest
{
    public required int Id { get; set; }
    public required string NewFileNameWithoutExtension { get; set; }
    public required string UpdateUserName { get; init; }
}