using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;

internal sealed class CreateImageFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, DirectoryNotFoundResult, FileAlreadyExistsResult>>
{
    public CreateImageFileAsyncOperation(string imageDirectoryPath, string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fileNameWithoutExtension = fileNameWithoutExtension;
        _imageData = imageData;
        _imageFileType = imageFileType;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fileNameWithoutExtension;
    private readonly byte[] _imageData;
    private readonly AllowedImageFileType _imageFileType;

    private bool _succeeeded = false;

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileAlreadyExistsResult>> ExecuteAsync()
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{_fileNameWithoutExtension}.{_imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = fullFileName };

        using MemoryStream memoryStream = new(_imageData);

        using Image image = await Image.LoadAsync(memoryStream);

        await image.SaveAsync(filePath);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string fullFileName = $"{_fileNameWithoutExtension}.{_imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");

        File.Delete(filePath);
    }
}