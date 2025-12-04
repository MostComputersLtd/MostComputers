using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
public interface IPromotionFileService
{
    Task<List<PromotionFileInfo>> GetAllAsync();
    Task<List<PromotionFileInfo>> GetAllByActivityAsync(bool active = true);
    Task<List<PromotionFileInfo>> GetByIdsAsync(IEnumerable<int> ids);
    Task<PromotionFileInfo?> GetByIdAsync(int id);
    Task<Stream?> GetFileDataByIdAsync(int fileInfoId);
    Task<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(CreatePromotionFileRequest createRequest);
    Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateAsync(UpdatePromotionFileRequest updateRequest);
    Task<OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult>> DeleteAsync(int promotionFileInfoId);
}