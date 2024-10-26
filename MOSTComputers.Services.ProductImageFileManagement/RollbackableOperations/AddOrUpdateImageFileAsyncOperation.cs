using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using SixLabors.ImageSharp;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp.Formats;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;

internal sealed class AddOrUpdateImageFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, DirectoryNotFoundResult>>
{
    public AddOrUpdateImageFileAsyncOperation(
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
    private bool _operationWasUpdate;
    private byte[]? _oldImageData;

    public async Task<OneOf<Success, DirectoryNotFoundResult>> ExecuteAsync()
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{_fileNameWithoutExtension}.{_imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (File.Exists(filePath))
        {
            _operationWasUpdate = true;

            _oldImageData = await File.ReadAllBytesAsync(filePath);
        }

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

        if (!_operationWasUpdate)
        {
            File.Delete(filePath);

            return;
        }

        if (!File.Exists(filePath)) throw new InvalidOperationException($"Inconsistent state: File to rollback, does not exist. Path: {filePath}");

        if (_oldImageData is null) throw new InvalidOperationException("Inconsistent state: Cannot rollback, because old data is lost");

        File.WriteAllBytes(filePath, _oldImageData);
    }
}