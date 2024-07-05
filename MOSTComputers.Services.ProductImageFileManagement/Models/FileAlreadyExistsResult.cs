namespace MOSTComputers.Services.ProductImageFileManagement.Models;

public readonly struct FileAlreadyExistsResult
{
    public required string FileName { get; init; }
}
