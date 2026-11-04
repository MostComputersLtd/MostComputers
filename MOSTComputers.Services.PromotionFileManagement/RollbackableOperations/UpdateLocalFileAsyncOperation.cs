using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class UpdateLocalFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileDoesntExistResult>>
{
    internal UpdateLocalFileAsyncOperation(
        string filePath, string fullFileName, byte[] fileData, CancellationToken cancellationToken = default)
    {
        _filePath = filePath;
        _fullFileName = fullFileName;
        _fileData = fileData;
        _cancellationToken = cancellationToken;
    }

    private readonly string _filePath;
    private readonly string _fullFileName;
    private readonly byte[] _fileData;
    private readonly CancellationToken _cancellationToken;

    private bool _destinationMayHaveChanged = false;

    private byte[]? _oldFileData = null;

    public async Task<OneOf<Success, FileDoesntExistResult>> ExecuteAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldFileData = await File.ReadAllBytesAsync(_filePath);

        await using FileStream stream = new(
            _filePath,
            FileMode.Open,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);

        _destinationMayHaveChanged = true;

        stream.SetLength(0);
        await stream.WriteAsync(_fileData, _cancellationToken);

        return new Success();
    }

    public void Rollback()
    {
        if (!_destinationMayHaveChanged) return;

        if (_oldFileData is null)
        {
            throw new InvalidOperationException($"Cannot rollback, because old file data is missing: {_filePath}");
        }

        File.WriteAllBytes(_filePath, _oldFileData);
    }
}
