using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOSTComputers.Services.ProductImageFileManagement.Models;

namespace MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;

internal sealed class RenameImageFileOperation : IRollbackableOperation<OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult>>
{
    public RenameImageFileOperation(
        string imageDirectoryPath,
        string fileNameWithoutExtension,
        string newFileNameWithoutExtension,
        AllowedImageFileType allowedImageFileType)
    {
        _imageDirectoryPath = imageDirectoryPath;
        _fileNameWithoutExtension = fileNameWithoutExtension;
        _newFileNameWithoutExtension = newFileNameWithoutExtension;
        _allowedImageFileType = allowedImageFileType;
    }

    private readonly string _imageDirectoryPath;
    private readonly string _fileNameWithoutExtension;
    private readonly string _newFileNameWithoutExtension;
    private readonly AllowedImageFileType _allowedImageFileType;

    private bool _succeeded = false;

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> Execute()
    {
        string fullFileName = $"{_fileNameWithoutExtension}.{_allowedImageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        string newFullFileName = $"{_newFileNameWithoutExtension}.{_allowedImageFileType.FileExtension}";

        string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        if (File.Exists(newFilePath)) return new FileAlreadyExistsResult() { FileName = newFullFileName };

        File.Move(filePath, newFilePath);

        _succeeded = true;

        return new Success();
    }

    public void Rollback()
    {
        if (!_succeeded) return;

        string fullFileName = $"{_fileNameWithoutExtension}.{_allowedImageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        string newFullFileName = $"{_newFileNameWithoutExtension}.{_allowedImageFileType.FileExtension}";

        string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        if (!File.Exists(newFilePath)) throw new FileNotFoundException($"Inconsistent state: Cannot rollback, because file is missing: {filePath}");

        File.Move(newFilePath, filePath);
    }
}