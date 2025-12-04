using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
internal sealed class AddOrUpdateImageFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult>>
{
    public AddOrUpdateImageFileAsyncOperation(
        string imageDirectoryPath,
        string fullFileName,
        byte[] imageData)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _imageData = imageData;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

    private bool _succeeded = false;
    private bool _operationWasUpdate;
    private byte[]? _oldImageData;

    public async Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult>> ExecuteAsync()
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            _operationWasUpdate = true;

            _oldImageData = await File.ReadAllBytesAsync(filePath);
        }
       
        string? contentType = GetContentTypeFromExtension(_fullFileName);

        if (!IsImageContentType(contentType))
        {
            return FileSaveFailureResult.FromUnsupportedContentType(new()
            {
                ContentType = contentType,
            });
        }

        using MemoryStream memoryStream = new(_imageData);

        try
        {
            using Image image = await Image.LoadAsync(memoryStream);

            await image.SaveAsync(filePath);

            _succeeded = true;

            return new Success();
        }
        catch (UnknownImageFormatException)
        {
            return FileSaveFailureResult.FromUnsupportedContentType(new()
            {
                ContentType = contentType,
            });
        }
        catch (InvalidImageContentException)
        {
            return FileSaveFailureResult.FromInvalidContent(new());
        }
    }

    public void Rollback()
    {
        if (!_succeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

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