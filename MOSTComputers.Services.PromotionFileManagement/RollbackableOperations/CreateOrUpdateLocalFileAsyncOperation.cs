using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateOrUpdateLocalFileAsyncOperation : IRollbackableOperationAsync<CreateOrUpdateOperationTypeEnum>
{
    internal CreateOrUpdateLocalFileAsyncOperation(
        string directoryPath, string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        _directoryPath = directoryPath;
        _fullFileName = fullFileName;
        _fileData = fileData;
        _cancellationToken = cancellationToken;
    }

    private readonly string _directoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _fileData;
    private readonly CancellationToken? _cancellationToken;

    private bool _succeeeded = false;

    private CreateOrUpdateOperationTypeEnum? _createOrUpdateOperationType = null;
    private byte[]? _oldFileData = null;

    public async Task<CreateOrUpdateOperationTypeEnum> ExecuteAsync()
    {
        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            _oldFileData = await File.ReadAllBytesAsync(filePath);

            if (_cancellationToken is null)
            {
                await File.WriteAllBytesAsync(filePath, _fileData);
            }
            else
            {
                await File.WriteAllBytesAsync(filePath, _fileData, _cancellationToken.Value);
            }

            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Update;

            _succeeeded = true;

            return CreateOrUpdateOperationTypeEnum.Update;
        }

        if (_cancellationToken is null)
        {
            await File.WriteAllBytesAsync(filePath, _fileData);
        }
        else
        {
            await File.WriteAllBytesAsync(filePath, _fileData, _cancellationToken.Value);
        }

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