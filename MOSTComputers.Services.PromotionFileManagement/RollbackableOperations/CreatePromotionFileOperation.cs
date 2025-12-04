using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreatePromotionFileOperation : IRollbackableOperation<OneOf<Success, FileAlreadyExistsResult>>
{
    internal CreatePromotionFileOperation(string imageDirectoryPath, string fullFileName, byte[] imageData)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _imageData = imageData;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

    private bool _succeeeded = false;

    public OneOf<Success, FileAlreadyExistsResult> Execute()
    {
        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = _fullFileName };

        File.WriteAllBytes(filePath, _imageData);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");
        }

        File.Delete(filePath);
    }
}