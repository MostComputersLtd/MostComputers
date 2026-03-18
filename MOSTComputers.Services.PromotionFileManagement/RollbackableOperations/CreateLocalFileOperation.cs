using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateLocalFileOperation : IRollbackableOperation<OneOf<Success, FileAlreadyExistsResult>>
{
    internal CreateLocalFileOperation(string directoryPath, string fullFileName, byte[] fileData)
    {
        _directoryPath = directoryPath;
        _fullFileName = fullFileName;
        _imageData = fileData;
    }

    private readonly string _directoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

    private bool _succeeeded = false;

    public OneOf<Success, FileAlreadyExistsResult> Execute()
    {
        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = _fullFileName };

        File.WriteAllBytes(filePath, _imageData);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");
        }

        File.Delete(filePath);
    }
}