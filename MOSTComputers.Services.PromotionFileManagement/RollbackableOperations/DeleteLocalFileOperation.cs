using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class DeleteLocalFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    internal DeleteLocalFileOperation(string filePath, string fullFileName)
    {
        _filePath = filePath;
        _fullFileName = fullFileName;
    }

    private readonly string _filePath;
    private readonly string _fullFileName;

    private bool _destinationMayHaveChanged = false;

    private byte[]? _oldFileData = null;

    public OneOf<Success, FileDoesntExistResult> Execute()
    {
        if (!File.Exists(_filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldFileData = File.ReadAllBytes(_filePath);

        File.Delete(_filePath);

        _destinationMayHaveChanged = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_destinationMayHaveChanged) return;

        if (File.Exists(_filePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be added already exists.");
        }

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {_filePath}");
        }

        File.WriteAllBytes(_filePath, _oldFileData);
    }
}
