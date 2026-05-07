using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IPromotionGroupService
{
    Task<List<PromotionGroup>> GetAllAsync();
    Task<PromotionGroup?> GetByIdAsync(int id);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionGroupCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServicePromotionGroupUpdateRequest updateRequest);
}