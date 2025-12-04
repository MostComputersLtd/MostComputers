using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
public interface IProductImageFileService
{
    Task<List<ProductImageFileData>> GetAllAsync();
    Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductImageFileData>> GetAllInProductAsync(int productId);
    Task<ProductImageFileData?> GetByIdAsync(int id);
    Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId);
    Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertFileAsync(ProductImageFileCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> ChangeFileAsync(ProductImageFileChangeRequest updateRequest);
    Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateFileAsync(ProductImageFileUpdateRequest updateRequest);
    Task<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> RenameFileAsync(ProductImageFileRenameRequest renameRequest);
    Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteAllFilesForProductAsync(int productId, string deleteUserName);
    Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(int id, string deleteUserName);
    Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(ProductImageFileData fileNameInfo, string deleteUserName);
    Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileByProductIdAndImageIdAsync(int productId, int imageId, string deleteUserName);
}