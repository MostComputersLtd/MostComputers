namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

public sealed class FileForImageUpsertRequest
{
    public int? CustomDisplayOrder { get; init; }
    public bool? Active { get; init; }
    public required string UpsertUserName { get; init; }
}