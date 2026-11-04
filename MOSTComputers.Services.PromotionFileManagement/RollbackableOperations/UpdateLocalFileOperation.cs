using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class UpdateLocalFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult>>
{
    internal UpdateLocalFileOperation(string filePath, string fullFileName, byte[] fileData)
    {
        _filePath = filePath;
        _fullFileName = fullFileName;
        _fileData = fileData;
    }

    private readonly string _filePath;
    private readonly string _fullFileName;
    private readonly byte[] _fileData;

    private bool _destinationMayHaveChanged = false;

    private byte[]? _oldFileData = null;

    public OneOf<Success, FileDoesntExistResult> Execute()
    {
        if (!File.Exists(_filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        _oldFileData = File.ReadAllBytes(_filePath);

        using FileStream fileStream = new(
            _filePath,
            FileMode.Open,
            FileAccess.Write,
            FileShare.Read);

        _destinationMayHaveChanged = true;

        fileStream.SetLength(0);
        fileStream.Write(_fileData);

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
