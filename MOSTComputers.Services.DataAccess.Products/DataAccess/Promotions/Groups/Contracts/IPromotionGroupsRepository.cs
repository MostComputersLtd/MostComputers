using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IPromotionGroupsRepository
{
    Task<List<PromotionGroup>> GetAllAsync();
    Task<PromotionGroup?> GetByIdAsync(int id);
    Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionGroupCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> UpdateAsync(PromotionGroupUpdateRequest updateRequest);
}