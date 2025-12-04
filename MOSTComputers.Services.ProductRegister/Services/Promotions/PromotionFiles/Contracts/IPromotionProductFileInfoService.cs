using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
internal interface IPromotionProductFileInfoService
{
    Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId);
    Task<int> GetCountOfAllForProductAsync(int productId);
    Task<bool> DoesExistForPromotionFileAsync(int promotionFileId);
    Task<PromotionProductFileInfo?> GetByIdAsync(int id);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionProductFileInfoCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServicePromotionProductFileInfoUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);
}