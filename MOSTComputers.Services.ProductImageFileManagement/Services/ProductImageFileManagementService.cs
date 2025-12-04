using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.IO;

namespace MOSTComputers.Services.ProductImageFileManagement.Services;
internal sealed class ProductImageFileManagementService : IProductImageFileManagementService
{
    public ProductImageFileManagementService(string imageDirectoryFullPath,
        ITransactionalFileManager transactionalFileManager)
    {
        _imageDirectoryPath = imageDirectoryFullPath;
        _transactionalFileManager = transactionalFileManager;
    }

    private readonly string _imageDirectoryPath;
    private readonly ITransactionalFileManager _transactionalFileManager;

    public OneOf<string[], DirectoryNotFoundResult> GetAllImageFileNames()
    {
        if (!Directory.Exists(_imageDirectoryPath))
        {
            return new DirectoryNotFoundResult();
        }

        return Directory.GetFiles(_imageDirectoryPath);
    }

    public List<byte[]> GetAllStartingWith(string startingChars)
    {
        if (string.IsNullOrEmpty(startingChars)) return new();

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_imageDirectoryPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<byte[]> output = new();

        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(_imageDirectoryPath, fileName);

            byte[] fileData = File.ReadAllBytes(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public List<Stream> GetAllStartingWithAsStreams(string startingChars)
    {
        if (string.IsNullOrEmpty(startingChars)) return new();

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_imageDirectoryPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<Stream> output = new();

        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(_imageDirectoryPath, fileName);

            FileStream fileData = File.OpenRead(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public OneOf<byte[], FileDoesntExistResult> GetImage(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        return File.ReadAllBytes(filePath);
    }

    public OneOf<Stream, FileDoesntExistResult> GetImageStream(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        return File.OpenRead(filePath);
    }

    public async Task<OneOf<byte[], FileDoesntExistResult>> GetImageAsync(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    public bool CheckIfImageFileExists(string? fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return false;

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        return File.Exists(filePath);
    }

    public async Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult>> AddImageAsync(
        string fullFileName, byte[] imageData)
    {
        CreateImageFileAsyncOperation createImageFileAsyncOperation = new(_imageDirectoryPath, fullFileName, imageData);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createImageFileAsyncOperation);
    }

    public async Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateImageAsync(
        string fullFileName, byte[] imageData)
    {
        UpdateImageFileAsyncOperation updateImageFileAsyncOperation = new(_imageDirectoryPath, fullFileName, imageData);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(updateImageFileAsyncOperation);
    }

    public async Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult>> AddOrUpdateImageAsync(
        string fullFileName, byte[] imageData)
    {
        AddOrUpdateImageFileAsyncOperation addOrUpdateImageFileAsyncOperation = new(_imageDirectoryPath, fullFileName, imageData);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(addOrUpdateImageFileAsyncOperation);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameImageFile(
        string fullFileName,
        string newFileNameWithoutExtension)
    {
        RenameImageFileOperation renameImageFileOperation
            = new(_imageDirectoryPath, fullFileName, newFileNameWithoutExtension);

        return _transactionalFileManager.EnlistOrExecuteOperation(renameImageFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        DeleteImageFileOperation deleteImageFileOperation = new(_imageDirectoryPath, fullFileName);

        return _transactionalFileManager.EnlistOrExecuteOperation(deleteImageFileOperation);
    }
}