using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class CreateLocalFileOperation : IRollbackableOperation<OneOf<Success, FileAlreadyExistsResult>>
{
    internal CreateLocalFileOperation(string filePath, string fullFileName, byte[] fileData)
    {
        _filePath = filePath;
        _fullFileName = fullFileName;
        _imageData = fileData;
    }

    private readonly string _filePath;
    private readonly string _fullFileName;
    private readonly byte[] _imageData;

    private bool _destinationMayHaveChanged = false;

    public OneOf<Success, FileAlreadyExistsResult> Execute()
    {
        if (File.Exists(_filePath)) return new FileAlreadyExistsResult() { FileName = _fullFileName };

        using FileStream stream = new(
            _filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096);

        _destinationMayHaveChanged = true;

        stream.Write(_imageData);

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
