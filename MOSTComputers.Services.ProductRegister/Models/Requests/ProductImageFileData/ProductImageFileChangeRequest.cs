namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;

public sealed class ProductImageFileChangeRequest
{
    public required int Id { get; set; }
    public required FileData NewFileData { get; set; }
    public required string UpdateUserName { get; init; }
}