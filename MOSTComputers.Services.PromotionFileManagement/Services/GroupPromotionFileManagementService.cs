using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.Services;
internal sealed class GroupPromotionFileManagementService : IGroupPromotionFileManagementService
{
    public GroupPromotionFileManagementService(
        string groupPromotionFileDirectoryFullPath,
        ITransactionalFileManager transactionalFileManager)
    {
        _groupPromotionFileDirectoryFullPath = groupPromotionFileDirectoryFullPath;
        _transactionalFileManager = transactionalFileManager;

        _promotionFileManagementService = new PromotionFileManagementService(
            _groupPromotionFileDirectoryFullPath,
            _transactionalFileManager);
    }

    private readonly string _groupPromotionFileDirectoryFullPath;

    private readonly ITransactionalFileManager _transactionalFileManager;

    private readonly PromotionFileManagementService _promotionFileManagementService;

    public byte[]? GetFile(string fullFileName)
    {
        return _promotionFileManagementService.GetFile(fullFileName);
    }

    public Task<byte[]?> GetFileAsync(string fullFileName)
    {
        return _promotionFileManagementService.GetFileAsync(fullFileName);
    }

    public Stream? GetFileStream(string fullFileName)
    {
        return _promotionFileManagementService.GetFileStream(fullFileName);
    }

    public bool DoesFileExist(string fullFileName)
    {
        return _promotionFileManagementService.DoesFileExist(fullFileName);
    }

    public OneOf<Success, FileAlreadyExistsResult> AddFile(string fullFileName, byte[] fileData)
    {
        return _promotionFileManagementService.AddFile(fullFileName, fileData);
    }

    public Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        return _promotionFileManagementService.AddFileAsync(fullFileName, fileData, cancellationToken);
    }

    public CreateOrUpdateOperationTypeEnum AddOrUpdateFile(string fullFileName, byte[] fileData)
    {
        return _promotionFileManagementService.AddOrUpdateFile(fullFileName, fileData);
    }

    public Task<CreateOrUpdateOperationTypeEnum> AddOrUpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        return _promotionFileManagementService.AddOrUpdateFileAsync(fullFileName, fileData, cancellationToken);
    }

    public OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData)
    {
        return _promotionFileManagementService.UpdateFile(fullFileName, fileData);
    }

    public Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null)
    {
        return _promotionFileManagementService.UpdateFileAsync(fullFileName, fileData, cancellationToken);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension)
    {
        return _promotionFileManagementService.RenameFile(fullFileName, newFileNameWithoutExtension);
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        return _promotionFileManagementService.DeleteFile(fullFileName);
    }
}