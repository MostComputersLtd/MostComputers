using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class UpdateLocalFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileDoesntExistResult>>
{
    internal UpdateLocalFileAsyncOperation(
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

    private byte[]? _oldFileData = null;

    public async Task<OneOf<Success, FileDoesntExistResult>> ExecuteAsync()
    {
        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldFileData = await File.ReadAllBytesAsync(filePath);

        if (_cancellationToken is null)
        {
            await File.WriteAllBytesAsync(filePath, _fileData);
        }
        else
        {
            await File.WriteAllBytesAsync(filePath, _fileData, _cancellationToken.Value);
        }

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string filePath = Path.Combine(_directoryPath, _fullFileName);

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {filePath}");
        }

        File.WriteAllBytes(filePath, _oldFileData);
    }
}