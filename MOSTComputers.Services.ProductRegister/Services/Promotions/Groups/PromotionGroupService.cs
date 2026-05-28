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

public sealed class PromotionGroupService : IPromotionGroupService
{
    private readonly IPromotionGroupsRepository _promotionGroupsRepository;
    private readonly IValidator<ServicePromotionGroupCreateRequest> _createRequestValidator;
    private readonly IValidator<ServicePromotionGroupUpdateRequest> _updateRequestValidator;

    public PromotionGroupService(IPromotionGroupsRepository promotionGroupsRepository,
        IValidator<ServicePromotionGroupCreateRequest> createRequestValidator,
        IValidator<ServicePromotionGroupUpdateRequest> updateRequestValidator)
    {
        _promotionGroupsRepository = promotionGroupsRepository;
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

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionGroupCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

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

        OneOf<int, UnexpectedFailureResult> result = await _promotionGroupsRepository.InsertAsync(innerCreateRequest);

        return result.Map<int, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServicePromotionGroupUpdateRequest updateRequest)
    {
        if (updateRequest.Id <= 0) return new NotFound();

        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

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

        OneOf<Success, NotFound> result = await _promotionGroupsRepository.UpdateAsync(innerUpdateRequest);

        return result.Map<Success, NotFound, ValidationResult>();
    }
}