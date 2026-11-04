using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateOrUpdateLocalFileAsyncOperation : IRollbackableOperationAsync
{
    internal CreateOrUpdateLocalFileAsyncOperation(
        string filePath, byte[] fileData, CancellationToken cancellationToken = default)
    {
        _filePath = filePath;
        _fileData = fileData;
        _cancellationToken = cancellationToken;
    }

    private readonly string _filePath;
    private readonly byte[] _fileData;
    private readonly CancellationToken _cancellationToken;

    private bool _destinationMayHaveChanged = false;

    private CreateOrUpdateOperationTypeEnum? _createOrUpdateOperationType = null;
    private byte[]? _oldFileData = null;

    public async Task ExecuteAsync()
    {
        if (File.Exists(_filePath))
        {
            _oldFileData = await File.ReadAllBytesAsync(_filePath, _cancellationToken);
            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Update;
        }
        else
        {
            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Create;
        }

        _destinationMayHaveChanged = true;

        await using FileStream fileStream = new(
            _filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);

        await fileStream.WriteAsync(_fileData, _cancellationToken);
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
