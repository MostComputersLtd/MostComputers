namespace MOSTComputers.Models.FileManagement.Models;

public readonly struct NotSupportedFileTypeResult
{
    public required string FileExtension { get; init; }
}