using MOSTComputers.Models.FileManagement.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
public interface ILocalFileManagementService
{
    string[] GetAllFileNames();
    List<byte[]> GetAllStartingWith(string startingChars);
    List<Stream> GetAllStartingWithAsStreams(string startingChars);
    byte[]? GetFile(string fullFileName);
    Task<byte[]?> GetFileAsync(string fullFileName);
    bool DoesFileExist(string fullFileName);
    OneOf<Success, FileAlreadyExistsResult> AddFile(string fullFileName, byte[] fileData);
    Task<OneOf<Success, FileAlreadyExistsResult>> AddFileAsync(string fullFileName, byte[] fileData, CancellationToken cancellationToken = default);
    void AddOrUpdateFile(string fullFileName, byte[] fileData);
    Task AddOrUpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken cancellationToken = default);
    OneOf<Success, FileDoesntExistResult> UpdateFile(string fullFileName, byte[] fileData);
    Task<OneOf<Success, FileDoesntExistResult>> UpdateFileAsync(string fullFileName, byte[] fileData, CancellationToken cancellationToken = default);
    OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameFile(string fullFileName, string newFileNameWithoutExtension);
    OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName);
    Stream? GetFileStream(string fullFileName);
}
