using MOSTComputers.Models.FileManagement.Models;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using System.Diagnostics.Metrics;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using SixLabors.ImageSharp;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;

internal sealed class UpdateImageFileAsyncOperation
    : IRollbackableOperationAsync<OneOf<Success, DirectoryNotFoundResult, FileDoesntExistResult>>
{
    public UpdateImageFileAsyncOperation(
        string imageDirectoryPath,
        string fileNameWithoutExtension,
        byte[] imageData,
        AllowedImageFileType imageFileType)
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

    private bool _succeeded = false;

    private byte[]? _oldImageData;

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileDoesntExistResult>> ExecuteAsync()
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{_fileNameWithoutExtension}.{_imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        _oldImageData = File.ReadAllBytes(filePath);

        using MemoryStream memoryStream = new(_imageData);

        using Image image = await Image.LoadAsync(memoryStream);

        await image.SaveAsync(filePath);

        _succeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeded) return;

        string fullFileName = $"{_fileNameWithoutExtension}.{_imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"Inconsistent state: Cannot rollback, because file is missing: {filePath}");

        if (_oldImageData is null) throw new InvalidOperationException("Inconsistent state: Cannot rollback, because old data is lost");

        File.WriteAllBytes(filePath, _oldImageData);
    }
}