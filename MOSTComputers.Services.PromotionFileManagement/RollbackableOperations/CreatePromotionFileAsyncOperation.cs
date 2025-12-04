using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreatePromotionFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileAlreadyExistsResult>>
{
    internal CreatePromotionFileAsyncOperation(
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

    public async Task<OneOf<Success, FileAlreadyExistsResult>> ExecuteAsync()
    {
        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = _fullFileName };

        if (_cancellationToken is null)
        {
            await File.WriteAllBytesAsync(filePath, _imageData);
        }
        else
        {
            await File.WriteAllBytesAsync(filePath, _imageData, _cancellationToken.Value);
        }

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