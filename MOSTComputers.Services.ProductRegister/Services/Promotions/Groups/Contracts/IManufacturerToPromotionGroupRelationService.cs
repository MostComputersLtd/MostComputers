using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IManufacturerToPromotionGroupRelationService
{
    Task<bool> DeleteByManufacturerIdAsync(int manufacturerId);
    Task<bool> DeleteByPromotionGroupIdAsync(int promotionGroupId);
    Task<List<ManufacturerToPromotionGroupRelation>> GetAllAsync();
    Task<ManufacturerToPromotionGroupRelation?> GetByManufacturerIdAsync(int manufacturerId);
    Task<ManufacturerToPromotionGroupRelation?> GetByPromotionGroupIdAsync(int promotionGroupId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertByManufacturerIdAsync(ManufacturerToPromotionGroupRelationUpsertRequest upsertRequest);
}