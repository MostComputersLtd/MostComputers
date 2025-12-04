using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
internal sealed class RenameImageFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult>>
{
    public RenameImageFileOperation(
        string imageDirectoryPath,
        string fullFileName,
        string newFileNameWithoutExtension)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fullFileName = fullFileName;
        _newFileNameWithoutExtension = newFileNameWithoutExtension;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fullFileName;
    private readonly string _newFileNameWithoutExtension;

    private bool _succeeded = false;

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> Execute()
    {
        string fileExtension = Path.GetExtension(_fullFileName);

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = _fullFileName };
        }

        string newFullFileName = $"{_newFileNameWithoutExtension}{fileExtension}";

        string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        if (File.Exists(newFilePath))
        {
            return new FileAlreadyExistsResult() { FileName = newFullFileName };
        }

        File.Move(filePath, newFilePath);

        _succeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeded) return;

        string filePath = Path.Combine(_imageDirectoryPath, _fullFileName);

        string fileExtension = Path.GetExtension(_fullFileName);

        string newFullFileName = $"{_newFileNameWithoutExtension}{fileExtension}";

        string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        if (!File.Exists(newFilePath))
        {
            throw new FileNotFoundException($"Inconsistent state: Cannot rollback, because file is missing: {filePath}");
        }

        File.Move(newFilePath, filePath);
    }
}