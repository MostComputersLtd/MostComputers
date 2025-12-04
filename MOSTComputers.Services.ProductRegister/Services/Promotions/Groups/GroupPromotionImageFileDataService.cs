using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;
internal sealed class GroupPromotionImageFileDataService : IGroupPromotionImageFileDataService
{
    public GroupPromotionImageFileDataService(
        IGroupPromotionImageFileDatasRepository groupPromotionImageFileDatasRepository,
        IValidator<GroupPromotionImageFileDataCreateRequest>? createRequestValidator = null,
        IValidator<GroupPromotionImageFileDataUpdateRequest>? updateRequestValidator = null)
    {
        _groupPromotionImageFileDatasRepository = groupPromotionImageFileDatasRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IGroupPromotionImageFileDatasRepository _groupPromotionImageFileDatasRepository;
    private readonly IValidator<GroupPromotionImageFileDataCreateRequest>? _createRequestValidator;
    private readonly IValidator<GroupPromotionImageFileDataUpdateRequest>? _updateRequestValidator;

    public async Task<List<GroupPromotionImageFileData>> GetAllAsync()
    {
        return await _groupPromotionImageFileDatasRepository.GetAllAsync();
    }

    public async Task<List<IGrouping<int, GroupPromotionImageFileData>>> GetAllInPromotionsAsync(List<int> promotionIds)
    {
        if (promotionIds.Count <= 0) return new();

        promotionIds = promotionIds.Distinct().ToList();

        return await _groupPromotionImageFileDatasRepository.GetAllInPromotionsAsync(promotionIds);
    }

    public async Task<List<GroupPromotionImageFileData>> GetAllInPromotionAsync(int promotionId)
    {
        return await _groupPromotionImageFileDatasRepository.GetAllInPromotionAsync(promotionId);
    }

    public async Task<GroupPromotionImageFileData?> GetByIdAsync(int id)
    {
        return await _groupPromotionImageFileDatasRepository.GetByIdAsync(id);
    }

    public async Task<GroupPromotionImageFileData?> GetByImageIdAsync(int imageId)
    {
        return await _groupPromotionImageFileDatasRepository.GetByImageIdAsync(imageId);
    }

    public async Task<OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageFileDataCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<int, ImageFileAlreadyExistsResult, UnexpectedFailureResult> result = await _groupPromotionImageFileDatasRepository.InsertAsync(createRequest);

        return result.Map<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(GroupPromotionImageFileDataUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, NotFound> result = await _groupPromotionImageFileDatasRepository.UpdateAsync(updateRequest);

        return result.Map<Success, NotFound, ValidationResult>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _groupPromotionImageFileDatasRepository.DeleteAsync(id);
    }

    public async Task<bool> DeleteByImageIdAsync(int imageId)
    {
        return await _groupPromotionImageFileDatasRepository.DeleteByImageIdAsync(imageId);
    }
}