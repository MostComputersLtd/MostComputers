using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
internal sealed class CreateImageFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult>>
{
    public CreateImageFileAsyncOperation(string imageDirectoryPath, string fullFileName, byte[] imageData)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _imageData = imageData;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

    private bool _succeeded = false;

    public async Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult>> ExecuteAsync()
    {
        string? contentType = GetContentTypeFromExtension(_fullFileName);

        if (!IsImageContentType(contentType))
        {
            return FileSaveFailureResult.FromUnsupportedContentType(new()
            {
                ContentType = contentType,
            });
        }

        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            return new FileAlreadyExistsResult() { FileName = _fullFileName };
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

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");
        }

        File.Delete(filePath);
    }
}