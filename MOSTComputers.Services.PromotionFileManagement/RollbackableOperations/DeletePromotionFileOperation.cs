using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class DeletePromotionFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    internal DeletePromotionFileOperation(string imageDirectoryPath, string fullFileName)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;

    private bool _succeeeded = false;

    private byte[]? _oldImageData = null;

    public OneOf<Success, FileDoesntExistResult> Execute()
    {
        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldImageData = File.ReadAllBytes(filePath);

        File.Delete(filePath);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be added already exists.");
        }

        if (_oldImageData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldImageData);
    }
}