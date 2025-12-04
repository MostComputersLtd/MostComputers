namespace MOSTComputers.Services.ProductRegister.Models.Requests;

public sealed class FileData
{
    public required string FileName { get; set; }
    public required byte[] Data { get; set; }
}