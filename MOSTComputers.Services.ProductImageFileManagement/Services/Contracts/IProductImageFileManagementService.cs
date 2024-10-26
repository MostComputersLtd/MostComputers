using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductImageFileManagement.Services;
public interface IProductImageFileManagementService
{
    Task<OneOf<Success, DirectoryNotFoundResult, FileAlreadyExistsResult>> AddImageAsync(string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType);
    Task<OneOf<Success, DirectoryNotFoundResult>> AddOrUpdateImageAsync(string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType);
    bool CheckIfImageFileExists(string fullFileName);
    OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName);
    OneOf<string[], DirectoryNotFoundResult> GetAllImageFileNames();
    Task<OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult>> GetImageAsync(string fullFileName);
    OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> GetImage(string fullFileName);
    Task<OneOf<Success, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateImageAsync(string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType);
    OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameImageFile(string fileNameWithoutExtension, string newFileNameWithoutExtension, AllowedImageFileType allowedImageFileType);
    List<byte[]> GetAllStartingWith(string startingChars);
    List<byte[]> GetAllStartingWithProductId(int productId);
    byte[]? GetByImageId(int imageId);
}