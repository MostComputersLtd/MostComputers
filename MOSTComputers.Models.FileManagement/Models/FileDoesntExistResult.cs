namespace MOSTComputers.Models.FileManagement.Models;

public readonly struct FileDoesntExistResult
{
    public required string FileName { get; init; }
}