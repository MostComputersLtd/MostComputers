using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
internal sealed class RenameLocalFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult>>
{
    internal RenameLocalFileOperation(string directoryPath, string currentFullFileName, string newFileNameWithoutExtension)
    {
        _directoryPath = directoryPath;
        _currentFullFileName = currentFullFileName;
        _newFileNameWithoutExtension = newFileNameWithoutExtension;
    }

    private readonly string _directoryPath;
    private readonly string _currentFullFileName;
    private readonly string _newFileNameWithoutExtension;

    private bool _succeeeded = false;

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> Execute()
    {
        string oldFilePath = Path.Combine(_directoryPath, _currentFullFileName);

        string? fileExtension = Path.GetExtension(oldFilePath);

        if (string.IsNullOrWhiteSpace(fileExtension)
            || !fileExtension.StartsWith('.')
            || !File.Exists(oldFilePath))
        {
            return new FileDoesntExistResult() { FileName = _currentFullFileName };
        }

        string newFileName = _newFileNameWithoutExtension + fileExtension;

        string newFilePath = Path.Combine(_directoryPath, newFileName);

        if (File.Exists(newFilePath))
        {
            return new FileAlreadyExistsResult() { FileName = _currentFullFileName };
        }

        File.Move(oldFilePath, newFilePath);

        _succeeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeeded) return;

        string oldFilePath = Path.Combine(_directoryPath, _currentFullFileName);

        if (File.Exists(oldFilePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file that was moved still exists at its old location");
        }

        string? fileExtension = Path.GetExtension(oldFilePath);

        if (string.IsNullOrWhiteSpace(fileExtension)
            || !fileExtension.StartsWith('.'))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, operation seems to have completed with invalid data, that would make it impossible to complete");
        }

        string newFileName = _newFileNameWithoutExtension + fileExtension;

        string newFilePath = Path.Combine(_directoryPath, newFileName);

        if (!File.Exists(newFilePath))
        {
            throw new InvalidOperationException("Inconsistent state: Rollback discovered, that a file to be moved does not exist");
        }

        File.Move(newFilePath, oldFilePath);
    }
}