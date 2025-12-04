using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
public interface IPromotionProductFileService
{
    Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId);
    Task<int> GetCountOfAllForProductAsync(int productId);
    Task<PromotionProductFileInfo?> GetByIdAsync(int id);
    Task<bool> DoesExistForPromotionFileAsync(int promotionFileId);
    Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionProductFileCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateAsync(ServicePromotionProductFileUpdateRequest updateRequest);
    Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteAsync(int id, string deleteUserName);
}