using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateOrUpdateLocalFileOperation : IRollbackableOperation
{
    internal CreateOrUpdateLocalFileOperation(string filePath, byte[] fileData)
    {
        _filePath = filePath;
        _fileData = fileData;
    }

    private readonly string _filePath;
    private readonly byte[] _fileData;

    private bool _destinationMayHaveChanged = false;

    private CreateOrUpdateOperationTypeEnum? _createOrUpdateOperationType = null;
    private byte[]? _oldFileData = null;

    public void Execute()
    {
        if (File.Exists(_filePath))
        {
            _oldFileData = File.ReadAllBytes(_filePath);
            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Update;
        }
        else
        {
            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Create;
        }

        _destinationMayHaveChanged = true;

        using FileStream fileStream = new(
            _filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.Read);

        fileStream.Write(_fileData);
    }

    public void Rollback()
    {
        if (!_destinationMayHaveChanged) return;

        if (_createOrUpdateOperationType == CreateOrUpdateOperationTypeEnum.Create)
        {
            File.Delete(_filePath);

            return;
        }

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {_filePath}");
        }

        File.WriteAllBytes(_filePath, _oldFileData);
    }
}
