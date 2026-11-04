using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class RenameLocalFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult>>
{
    internal RenameLocalFileOperation(
        string currentFilePath, string currentFullFileName, string newDirectoryPath, string newFileNameWithoutExtension)
    {
        _currentFilePath = currentFilePath;
        _newDirectoryPath = newDirectoryPath;
        _currentFullFileName = currentFullFileName;
        _newFileNameWithoutExtension = newFileNameWithoutExtension;
    }

    private readonly string _currentFilePath;
    private readonly string _currentFullFileName;
    private readonly string _newDirectoryPath;
    private readonly string _newFileNameWithoutExtension;

    private bool _destinationMayHaveChanged = false;

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> Execute()
    {
        string? fileExtension = Path.GetExtension(_currentFilePath);

        if (string.IsNullOrWhiteSpace(fileExtension)
            || !fileExtension.StartsWith('.')
            || !File.Exists(_currentFilePath))
        {
            return new FileDoesntExistResult() { FileName = _currentFullFileName };
        }

        string newFileName = _newFileNameWithoutExtension + fileExtension;

        string newFilePath = Path.Combine(_newDirectoryPath, newFileName);

        if (File.Exists(newFilePath))
        {
            return new FileAlreadyExistsResult() { FileName = _currentFullFileName };
        }

        File.Move(_currentFilePath, newFilePath);

        _destinationMayHaveChanged = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_destinationMayHaveChanged) return;

        if (File.Exists(_currentFilePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file that was moved still exists at its old location");
        }

        string? fileExtension = Path.GetExtension(_currentFilePath);

        if (string.IsNullOrWhiteSpace(fileExtension)
            || !fileExtension.StartsWith('.'))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, operation seems to have completed with invalid data, that would make it impossible to complete");
        }

        string newFileName = _newFileNameWithoutExtension + fileExtension;

        string newFilePath = Path.Combine(_newDirectoryPath, newFileName);

        if (!File.Exists(newFilePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be moved does not exist");
        }

        File.Move(newFilePath, _currentFilePath);
    }
}
