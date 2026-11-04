using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.Services;
public class LocalFileManagementService : ILocalFileManagementService
{
    public LocalFileManagementService(
        string directoryFullPath,
        ITransactionalFileManager transactionalFileManager)
    {
        _directoryFullPath = directoryFullPath;
        _transactionalFileManager = transactionalFileManager;
    }

    private readonly string _directoryFullPath;

    private readonly ITransactionalFileManager _transactionalFileManager;

    public string[] GetAllFileNames()
    {
        return Directory.GetFiles(_directoryFullPath);
    }

    public List<byte[]> GetAllStartingWith(string startingChars)
    {
        if (string.IsNullOrEmpty(startingChars)) return new();

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_directoryFullPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<byte[]> output = new();

        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(_directoryFullPath, fileName);

            byte[] fileData = File.ReadAllBytes(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public List<Stream> GetAllStartingWithAsStreams(string startingChars)
    {
        if (string.IsNullOrEmpty(startingChars)) return new();

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_directoryFullPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<Stream> output = new();

        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(_directoryFullPath, fileName);

            FileStream fileData = File.OpenRead(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public byte[]? GetFile(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        return File.ReadAllBytes(filePath);
    }

    public virtual Stream? GetFileStream(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        return new FileStream(
            filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
    }

    public async Task<byte[]?> GetFileAsync(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        return await File.ReadAllBytesAsync(filePath);
    }

    public bool DoesFileExist(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return false;

        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        return File.Exists(filePath);
    }

    public OneOf<Success, FileAlreadyExistsResult> AddFile(string fullFileName, byte[] fileData)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        CreateLocalFileOperation createLocalFileOperation = new(filePath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(createLocalFileOperation);
    }

    public async Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(
        string fullFileName, byte[] fileData, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        CreateLocalFileAsyncOperation createLocalFileAsyncOperation
            = new(filePath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createLocalFileAsyncOperation);
    }

    public void AddOrUpdateFile(string fullFileName, byte[] fileData)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        CreateOrUpdateLocalFileOperation createOrUpdateLocalFileOperation = new(filePath, fileData);

        _transactionalFileManager.EnlistOrExecuteOperation(createOrUpdateLocalFileOperation);
    }

    public async Task AddOrUpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        CreateOrUpdateLocalFileAsyncOperation createOrUpdateLocalFileAsyncOperation
            = new(filePath, fileData, cancellationToken);

        await _transactionalFileManager.EnlistOrExecuteOperationAsync(createOrUpdateLocalFileAsyncOperation);
    }

    public OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        UpdateLocalFileOperation updateLocalFileOperation = new(filePath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(updateLocalFileOperation);
    }

    public async Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        UpdateLocalFileAsyncOperation updateLocalFileAsyncOperation
            = new(filePath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(updateLocalFileAsyncOperation);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        RenameLocalFileOperation renameLocalFileOperation = new(filePath, fullFileName, _directoryFullPath, newFileNameWithoutExtension);

        return _transactionalFileManager.EnlistOrExecuteOperation(renameLocalFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        string filePath = Path.Combine(_directoryFullPath, fullFileName);

        DeleteLocalFileOperation deleteLocalFileOperation = new(filePath, fullFileName);

        return _transactionalFileManager.EnlistOrExecuteOperation(deleteLocalFileOperation);
    }
}
