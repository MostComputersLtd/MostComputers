using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
internal sealed class DeleteImageFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    public DeleteImageFileOperation(string imageDirectoryPath, string fullFileName)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;

    private bool _succeeded = false;

    private byte[]? _oldImageData;

    public OneOf<Success, FileDoesntExistResult> Execute()
    {
        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldImageData = File.ReadAllBytes(filePath);

        File.Delete(filePath);

        _succeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath)) throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be added already exists.");

        if (_oldImageData is null) throw new InvalidOperationException("Inconsistent state: Cannot rollback, because old data is lost");

        using MemoryStream memoryStream = new(_oldImageData);

        using Image image = Image.Load(memoryStream);

        image.Save(filePath);
    }
}