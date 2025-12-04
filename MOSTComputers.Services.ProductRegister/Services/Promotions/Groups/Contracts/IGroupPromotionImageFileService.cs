using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IGroupPromotionImageFileService
{
    Task<List<GroupPromotionImageFileData>> GetAllAsync();
    Task<List<GroupPromotionImageFileData>> GetAllInPromotionAsync(int promotionId);
    Task<List<GroupPromotionImageFile>> GetAllInPromotionWithFilesAsync(int promotionId);
    Task<GroupPromotionImageFileData?> GetByIdAsync(int id);
    Task<GroupPromotionImageFile?> GetByIdWithFileAsync(int id);
    Task<GroupPromotionImageFileData?> GetByImageIdAsync(int imageId);
    Task<GroupPromotionImageFile?> GetByImageIdWithFileAsync(int imageId);
    Task<OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageFileDataCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult>> UpdateAsync(GroupPromotionImageFileDataUpdateRequest updateRequest);
    Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteAsync(int id);
    Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteByImageIdAsync(int imageId);
}