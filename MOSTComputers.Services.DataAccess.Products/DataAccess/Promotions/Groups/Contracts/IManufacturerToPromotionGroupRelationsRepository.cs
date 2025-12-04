using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IManufacturerToPromotionGroupRelationsRepository
{
    Task<List<ManufacturerToPromotionGroupRelation>> GetAllAsync();
    Task<ManufacturerToPromotionGroupRelation?> GetByManufacturerIdAsync(int manufacturerId);
    Task<ManufacturerToPromotionGroupRelation?> GetByPromotionGroupIdAsync(int promotionGroupId);
    Task<OneOf<Success, UnexpectedFailureResult>> UpsertByManufacturerIdAsync(ManufacturerToPromotionGroupRelationUpsertRequest upsertRequest);
    Task<bool> DeleteByManufacturerIdAsync(int manufacturerId);
    Task<bool> DeleteByPromotionGroupIdAsync(int promotionGroupId);
}