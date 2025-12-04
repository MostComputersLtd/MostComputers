using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.PromotionFileManagement.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
public interface IGroupPromotionFileManagementService
{
    OneOf<Success, FileAlreadyExistsResult> AddFile(string fullFileName, byte[] fileData);
    Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null);
    CreateOrUpdateOperationTypeEnum AddOrUpdateFile(string fullFileName, byte[] fileData);
    Task<CreateOrUpdateOperationTypeEnum> AddOrUpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null);
    OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName);
    bool DoesFileExist(string fullFileName);
    byte[]? GetFile(string fullFileName);
    Task<byte[]?> GetFileAsync(string fullFileName);
    Stream? GetFileStream(string fullFileName);
    OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension);
    OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData);
    Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken? cancellationToken = null);
}