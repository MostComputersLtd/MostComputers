using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateLocalFileAsyncOperation : IRollbackableOperationAsync<OneOf<Success, FileAlreadyExistsResult>>
{
    internal CreateLocalFileAsyncOperation(
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

    public async Task<OneOf<Success, FileAlreadyExistsResult>> ExecuteAsync()
    {
        if (File.Exists(_filePath))
        {
            return new FileAlreadyExistsResult() { FileName = _fullFileName };
        }

        await using FileStream stream = new(
            _filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);

        _destinationMayHaveChanged = true;

        await stream.WriteAsync(_fileData, _cancellationToken);

        return new Success();
    }

    public void Rollback()
    {
        if (!_destinationMayHaveChanged) return;

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Cannot rollback, because file is missing: {_filePath}");
        }

        File.Delete(_filePath);
    }
}
