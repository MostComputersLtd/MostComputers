using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.RollbackableOperations;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.Services;
internal sealed class PromotionFileManagementService : IPromotionFileManagementService
{
    public PromotionFileManagementService(
        string promotionFileDirectoryFullPath,
        ITransactionalFileManager transactionalFileManager)
    {
        _promotionFileDirectoryFullPath = promotionFileDirectoryFullPath;
        _transactionalFileManager = transactionalFileManager;
    }

    private readonly string _promotionFileDirectoryFullPath;

    private readonly ITransactionalFileManager _transactionalFileManager;

    public byte[]? GetFile(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_promotionFileDirectoryFullPath, fullFileName);

        return File.ReadAllBytes(filePath);
    }

    public Stream? GetFileStream(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_promotionFileDirectoryFullPath, fullFileName);

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
    }

    public async Task<byte[]?> GetFileAsync(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return null;

        string filePath = Path.Combine(_promotionFileDirectoryFullPath, fullFileName);

        return await File.ReadAllBytesAsync(filePath);
    }

    public bool DoesFileExist(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName)) return false;

        string filePath = Path.Combine(_promotionFileDirectoryFullPath, fullFileName);

        return File.Exists(filePath);
    }

    public OneOf<Success, FileAlreadyExistsResult> AddFile(string fullFileName, byte[] fileData)
    {
        CreatePromotionFileOperation createPromotionFileOperation = new(_promotionFileDirectoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(createPromotionFileOperation);
    }

    public async Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        CreatePromotionFileAsyncOperation createPromotionFileOperation
            = new(_promotionFileDirectoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createPromotionFileOperation);
    }

    public CreateOrUpdateOperationTypeEnum AddOrUpdateFile(string fullFileName, byte[] fileData)
    {
        CreateOrUpdatePromotionFileOperation createOrUpdatePromotionFileOperation = new(_promotionFileDirectoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(createOrUpdatePromotionFileOperation);
    }

    public async Task<CreateOrUpdateOperationTypeEnum> AddOrUpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        CreateOrUpdatePromotionFileAsyncOperation createOrUpdatePromotionFileOperation
            = new(_promotionFileDirectoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createOrUpdatePromotionFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData)
    {
        UpdatePromotionFileOperation updatePromotionFileOperation = new(_promotionFileDirectoryFullPath, fullFileName, fileData);

        return _transactionalFileManager.EnlistOrExecuteOperation(updatePromotionFileOperation);
    }

    public async Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(
        string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        UpdatePromotionFileAsyncOperation updatePromotionFileOperation
            = new(_promotionFileDirectoryFullPath, fullFileName, fileData, cancellationToken);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(updatePromotionFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension)
    {
        RenamePromotionFileOperation renamePromotionFileOperation = new(_promotionFileDirectoryFullPath, fullFileName, newFileNameWithoutExtension);

        return _transactionalFileManager.EnlistOrExecuteOperation(renamePromotionFileOperation);
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        DeletePromotionFileOperation deletePromotionFileOperation = new(_promotionFileDirectoryFullPath, fullFileName);

        return _transactionalFileManager.EnlistOrExecuteOperation(deletePromotionFileOperation);
    }
}