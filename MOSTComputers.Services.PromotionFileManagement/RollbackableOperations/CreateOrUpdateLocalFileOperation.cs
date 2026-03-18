using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateOrUpdateLocalFileOperation : IRollbackableOperation<CreateOrUpdateOperationTypeEnum>
{
    internal CreateOrUpdateLocalFileOperation(string directoryPath, string fullFileName, byte[] fileData)
    {
        _directoryPath = directoryPath;
        _fullFileName = fullFileName;
        _fileData = fileData;
    }

    private readonly string _directoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _fileData;

    private bool _succeeeded = false;

    private CreateOrUpdateOperationTypeEnum? _createOrUpdateOperationType = null;
    private byte[]? _oldFileData = null;

    public CreateOrUpdateOperationTypeEnum Execute()
    {
        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            _oldFileData = File.ReadAllBytes(filePath);

            File.WriteAllBytes(filePath, _fileData);

            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Update;

            _succeeeded = true;

            return CreateOrUpdateOperationTypeEnum.Update;
        }

        File.WriteAllBytes(filePath, _fileData);

        _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Create;

        _succeeeded = true;

        return CreateOrUpdateOperationTypeEnum.Create;
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (_createOrUpdateOperationType == CreateOrUpdateOperationTypeEnum.Create)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");
            }

            File.Delete(filePath);

            return;
        }

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldFileData);
    }
}
