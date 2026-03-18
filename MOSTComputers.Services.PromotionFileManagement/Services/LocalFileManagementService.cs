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

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
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
        CreateLocalFileOperation createLocalFileOperation = new(_directoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(createLocalFileOperation);
    }

    public async Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        CreateLocalFileAsyncOperation createLocalFileAsyncOperation
            = new(_directoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createLocalFileAsyncOperation);
    }

    public CreateOrUpdateOperationTypeEnum AddOrUpdateFile(string fullFileName, byte[] fileData)
    {
        CreateOrUpdateLocalFileOperation createOrUpdateLocalFileOperation = new(_directoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(createOrUpdateLocalFileOperation);
    }

    public async Task<CreateOrUpdateOperationTypeEnum> AddOrUpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        CreateOrUpdateLocalFileAsyncOperation createOrUpdateLocalFileAsyncOperation
            = new(_directoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createOrUpdateLocalFileAsyncOperation);
    }

    public OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData)
    {
        UpdateLocalFileOperation updateLocalFileOperation = new(_directoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(updateLocalFileOperation);
    }

    public async Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        UpdateLocalFileAsyncOperation updateLocalFileAsyncOperation
            = new(_directoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(updateLocalFileAsyncOperation);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension)
    {
        RenameLocalFileOperation renameLocalFileOperation = new(_directoryFullPath, fullFileName, newFileNameWithoutExtension);

        return _transactionalFileManager.EnlistOrExecuteOperation(renameLocalFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        DeleteLocalFileOperation deleteLocalFileOperation = new(_directoryFullPath, fullFileName);

        return _transactionalFileManager.EnlistOrExecuteOperation(deleteLocalFileOperation);
    }
}