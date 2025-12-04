using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;
internal sealed class ManufacturerToPromotionGroupRelationService : IManufacturerToPromotionGroupRelationService
{
    public ManufacturerToPromotionGroupRelationService(
        IManufacturerToPromotionGroupRelationsRepository manufacturerToPromotionGroupRelationsRepository,
        IValidator<ManufacturerToPromotionGroupRelationUpsertRequest>? upsertRequestValidator = null)
    {
        _manufacturerToPromotionGroupRelationsRepository = manufacturerToPromotionGroupRelationsRepository;
        _upsertRequestValidator = upsertRequestValidator;
    }

    private readonly IManufacturerToPromotionGroupRelationsRepository _manufacturerToPromotionGroupRelationsRepository;
    private readonly IValidator<ManufacturerToPromotionGroupRelationUpsertRequest>? _upsertRequestValidator;

    public async Task<List<ManufacturerToPromotionGroupRelation>> GetAllAsync()
    {
        return await _manufacturerToPromotionGroupRelationsRepository.GetAllAsync();
    }

    public async Task<ManufacturerToPromotionGroupRelation?> GetByManufacturerIdAsync(int manufacturerId)
    {
        return await _manufacturerToPromotionGroupRelationsRepository.GetByManufacturerIdAsync(manufacturerId);
    }

    public async Task<ManufacturerToPromotionGroupRelation?> GetByPromotionGroupIdAsync(int promotionGroupId)
    {
        return await _manufacturerToPromotionGroupRelationsRepository.GetByPromotionGroupIdAsync(promotionGroupId);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertByManufacturerIdAsync(ManufacturerToPromotionGroupRelationUpsertRequest upsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_upsertRequestValidator, upsertRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> result
            = await _manufacturerToPromotionGroupRelationsRepository.UpsertByManufacturerIdAsync(upsertRequest);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<bool> DeleteByManufacturerIdAsync(int manufacturerId)
    {
        if (manufacturerId <= 0) return false;

        return await _manufacturerToPromotionGroupRelationsRepository.DeleteByManufacturerIdAsync(manufacturerId);
    }

    public async Task<bool> DeleteByPromotionGroupIdAsync(int promotionGroupId)
    {
        if (promotionGroupId <= 0) return false;

        return await _manufacturerToPromotionGroupRelationsRepository.DeleteByPromotionGroupIdAsync(promotionGroupId);
    }
}