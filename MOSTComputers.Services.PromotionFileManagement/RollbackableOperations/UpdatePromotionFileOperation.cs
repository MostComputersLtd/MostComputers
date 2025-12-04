using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class UpdatePromotionFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    internal UpdatePromotionFileOperation(string imageDirectoryPath, string fullFileName, byte[] imageData)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _imageData = imageData;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

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

        File.WriteAllBytes(filePath, _imageData);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (_oldImageData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldImageData);
    }
}