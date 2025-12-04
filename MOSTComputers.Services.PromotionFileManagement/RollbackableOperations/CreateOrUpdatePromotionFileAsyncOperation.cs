using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateOrUpdatePromotionFileAsyncOperation : IRollbackableOperationAsync<CreateOrUpdateOperationTypeEnum>
{
    internal CreateOrUpdatePromotionFileAsyncOperation(
        string imageDirectoryPath, string fullFileName, byte[] imageData, CancellationToken? cancellationToken = null)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _imageData = imageData;
        _cancellationToken = cancellationToken;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;
    private readonly CancellationToken? _cancellationToken;

    private bool _succeeeded = false;

    private CreateOrUpdateOperationTypeEnum? _createOrUpdateOperationType = null;
    private byte[]? _oldImageData = null;

    public async Task<CreateOrUpdateOperationTypeEnum> ExecuteAsync()
    {
        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath))
        {
            _oldImageData = await File.ReadAllBytesAsync(filePath);

            if (_cancellationToken is null)
            {
                await File.WriteAllBytesAsync(filePath, _imageData);
            }
            else
            {
                await File.WriteAllBytesAsync(filePath, _imageData, _cancellationToken.Value);
            }

            _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Update;

            _succeeeded = true;

            return CreateOrUpdateOperationTypeEnum.Update;
        }

        if (_cancellationToken is null)
        {
            await File.WriteAllBytesAsync(filePath, _imageData);
        }
        else
        {
            await File.WriteAllBytesAsync(filePath, _imageData, _cancellationToken.Value);
        }

        _createOrUpdateOperationType = CreateOrUpdateOperationTypeEnum.Create;

        _succeeeded = true;

        return CreateOrUpdateOperationTypeEnum.Create;
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (_createOrUpdateOperationType == CreateOrUpdateOperationTypeEnum.Create)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot rollback, because file is missing: {filePath}");
            }

            File.Delete(filePath);

            return;
        }

        if (_oldImageData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldImageData);
    }
}