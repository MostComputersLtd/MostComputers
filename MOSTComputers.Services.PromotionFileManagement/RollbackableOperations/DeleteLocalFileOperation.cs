using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class DeleteLocalFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    internal DeleteLocalFileOperation(string directoryPath, string fullFileName)
    {
        _directoryPath = directoryPath;
        _fullFileName = fullFileName;
    }

    private readonly string _directoryPath;
    private readonly string _fullFileName;

    private bool _succeeeded = false;

    private byte[]? _oldFileData = null;

    public OneOf<Success, FileDoesntExistResult> Execute()
    {
        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldFileData = File.ReadAllBytes(filePath);

        File.Delete(filePath);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be added already exists.");
        }

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldFileData);
    }
}