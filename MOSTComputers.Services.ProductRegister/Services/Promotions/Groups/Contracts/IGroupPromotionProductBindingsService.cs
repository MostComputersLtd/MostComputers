using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

public interface IGroupPromotionProductBindingsService
{
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
    Task<OneOf<Success, NotFound>> UpsertAllAsync(int promotionId, List<int>? relatedProductIds);
}