using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IGroupPromotionImageCrudService
{
    Task<OneOf<Success, NotFound>> DeleteAsync(int id);
    Task<List<GroupPromotionImage>> GetAllInPromotionAsync(int groupPromotionId);
    Task<List<GroupPromotionImageWithoutFile>> GetAllInPromotionWithoutFilesAsync(int groupPromotionId);
    Task<List<GroupPromotionImageWithoutFile>> GetAllWithoutFilesAsync();
    Task<GroupPromotionImage?> GetByIdAsync(int id);
    Task<List<GroupPromotionImage>> GetByIdsAsync(IEnumerable<int> ids);
    Task<List<GroupPromotionImageWithoutFile>> GetByIdsWithoutFilesAsync(IEnumerable<int> ids);
    Task<GroupPromotionImageWithoutFile?> GetByIdWithoutFileAsync(int id);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(GroupPromotionImageUpdateRequest updateRequest);
}