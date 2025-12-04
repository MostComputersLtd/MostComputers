using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;

namespace MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
public interface IProductImageFileManagementService
{
    OneOf<string[], DirectoryNotFoundResult> GetAllImageFileNames();
    List<byte[]> GetAllStartingWith(string startingChars);
    OneOf<byte[], FileDoesntExistResult> GetImage(string fullFileName);
    Task<OneOf<byte[], FileDoesntExistResult>> GetImageAsync(string fullFileName);
    bool CheckIfImageFileExists(string? fullFileName);
    Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult>> AddImageAsync(string fullFileName, byte[] imageData);
    Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateImageAsync(string fullFileName, byte[] imageData);
    Task<OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult>> AddOrUpdateImageAsync(string fullFileName, byte[] imageData);
    OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameImageFile(string fullFileName, string newFileNameWithoutExtension);
    OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName);
    List<Stream> GetAllStartingWithAsStreams(string startingChars);
    OneOf<Stream, FileDoesntExistResult> GetImageStream(string fullFileName);
}