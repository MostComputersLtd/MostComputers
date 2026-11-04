using System.Transactions;
using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;

public sealed class PromotionGroupService : IPromotionGroupService
{
    private readonly IPromotionGroupsRepository _promotionGroupsRepository;
    private readonly IManufacturerToPromotionGroupRelationService _manufacturerToPromotionGroupRelationService;
    private readonly IValidator<ServicePromotionGroupCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServicePromotionGroupUpdateRequest>? _updateRequestValidator;

    public PromotionGroupService(
        IPromotionGroupsRepository promotionGroupsRepository,
        IManufacturerToPromotionGroupRelationService manufacturerToPromotionGroupRelationService,
        IValidator<ServicePromotionGroupCreateRequest>? createRequestValidator = null,
        IValidator<ServicePromotionGroupUpdateRequest>? updateRequestValidator = null)
    {
        _promotionGroupsRepository = promotionGroupsRepository;
        _manufacturerToPromotionGroupRelationService = manufacturerToPromotionGroupRelationService;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    public Task<List<PromotionGroup>> GetAllAsync()
    {
        return _promotionGroupsRepository.GetAllAsync();
    }

    public Task<PromotionGroup?> GetByIdAsync(int id)
    {
        return _promotionGroupsRepository.GetByIdAsync(id);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(
        ServicePromotionGroupCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        PromotionGroupCreateRequest innerCreateRequest = new()
        {
            Name = createRequest.Name,
            Header = createRequest.Header,
            LogoImage = createRequest.Logo?.Image,
            LogoContentType = createRequest.Logo?.ContentType,
            IsDefault = createRequest.IsDefault,
            DisplayOrder = createRequest.DisplayOrder,
            ShowEmptyForLogged = createRequest.ShowEmptyForLogged,
            ShowEmptyForNonLogged = createRequest.ShowEmptyForNonLogged,
        };

        OneOf<int, UnexpectedFailureResult> createPromotionGroupResult = await _promotionGroupsRepository.InsertAsync(innerCreateRequest);

        if (!createPromotionGroupResult.IsT0)
        {
            return createPromotionGroupResult.Map<int, ValidationResult, UnexpectedFailureResult>();
        }

        int promotionGroupId = createPromotionGroupResult.AsT0;

        if (createRequest.RelatedManufacturerId == null)
        {
            transactionScope.Complete();

            return promotionGroupId;
        }

        int relatedManufacturerId = createRequest.RelatedManufacturerId.Value;

        ManufacturerToPromotionGroupRelation? existingRelation
            = await _manufacturerToPromotionGroupRelationService.GetByManufacturerIdAsync(relatedManufacturerId);

        if (existingRelation != null)
        {
            ValidationFailure repeatedIdError = new(
                nameof(createRequest.RelatedManufacturerId),
                "Manufacturer is already related to a different promotion group.");

            return new ValidationResult([repeatedIdError]);
        }

        ManufacturerToPromotionGroupRelationUpsertRequest createRelationRequest = new()
        {
            ManufacturerId = relatedManufacturerId,
            PromotionGroupId = promotionGroupId,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> createRelationResult
            = await _manufacturerToPromotionGroupRelationService.UpsertByPromotionGroupIdAsync(createRelationRequest);

        if (!createRelationResult.IsT0)
        {
            return createRelationResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
                success => promotionGroupId,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        transactionScope.Complete();

        return promotionGroupId;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> UpdateAsync(
        ServicePromotionGroupUpdateRequest updateRequest)
    {
        if (updateRequest.Id <= 0) return new NotFound();

        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        PromotionGroupUpdateRequest innerUpdateRequest = new()
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            Header = updateRequest.Header,
            LogoImage = updateRequest.Logo?.Image,
            LogoContentType = updateRequest.Logo?.ContentType,
            IsDefault = updateRequest.IsDefault,
            DisplayOrder = updateRequest.DisplayOrder,
            ShowEmptyForLogged = updateRequest.ShowEmptyForLogged,
            ShowEmptyForNonLogged = updateRequest.ShowEmptyForNonLogged,
        };

        Console.WriteLine($"UPDATE: {updateRequest.Logo?.Image.Length}, {updateRequest.Logo?.ContentType}");

        OneOf<Success, NotFound> updatePromotionGroupResult = await _promotionGroupsRepository.UpdateAsync(innerUpdateRequest);

        if (!updatePromotionGroupResult.IsT0)
        {
            return updatePromotionGroupResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
        }

        if (updateRequest.RelatedManufacturerId == null)
        {
            ManufacturerToPromotionGroupRelation? existingPromotionGroupRelation
                = await _manufacturerToPromotionGroupRelationService.GetByPromotionGroupIdAsync(updateRequest.Id);

            if (existingPromotionGroupRelation != null)
            {
                bool deleteExistingRelationResult
                    = await _manufacturerToPromotionGroupRelationService.DeleteByPromotionGroupIdAsync(updateRequest.Id);

                if (!deleteExistingRelationResult) return new UnexpectedFailureResult();
            }

            transactionScope.Complete();

            return updatePromotionGroupResult.AsT0;
        }

        int relatedManufacturerId = updateRequest.RelatedManufacturerId.Value;

        ManufacturerToPromotionGroupRelation? existingManufacturerRelation
            = await _manufacturerToPromotionGroupRelationService.GetByManufacturerIdAsync(relatedManufacturerId);

        if (existingManufacturerRelation != null)
        {
            ValidationFailure repeatedIdError = new(
                nameof(updateRequest.RelatedManufacturerId),
                "Manufacturer is already related to a different promotion group.");

            return new ValidationResult([repeatedIdError]);
        }

        ManufacturerToPromotionGroupRelationUpsertRequest upsertRelationRequest = new()
        {
            ManufacturerId = relatedManufacturerId,
            PromotionGroupId = updateRequest.Id,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertRelationResult
            = await _manufacturerToPromotionGroupRelationService.UpsertByPromotionGroupIdAsync(upsertRelationRequest);

        if (!upsertRelationResult.IsT0)
        {
            return upsertRelationResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
        }

        transactionScope.Complete();

        return new Success();
    }
}
