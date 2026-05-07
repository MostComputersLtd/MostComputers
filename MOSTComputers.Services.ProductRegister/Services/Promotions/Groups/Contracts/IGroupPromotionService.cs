using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IGroupPromotionService
{
    Task<List<GroupPromotionContent>> GetAllActiveAsync();
    Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds);
    Task<List<GroupPromotionContent>> GetAllAsync();
    Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds);
    Task<GroupPromotionContent?> GetByIdAsync(int id);
    Task<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(ServiceGroupPromotionContentCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>> UpdateAsync(ServiceGroupPromotionContentUpdateRequest updateRequest);
}