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

public sealed class GroupPromotionImageCrudService : IGroupPromotionImageCrudService
{
    private readonly IGroupPromotionImagesRepository _groupPromotionImagesRepository;
    private readonly IValidator<GroupPromotionImageCreateRequest> _createRequestValidator;
    private readonly IValidator<GroupPromotionImageUpdateRequest> _updateRequestValidator;

    public GroupPromotionImageCrudService(IGroupPromotionImagesRepository groupPromotionImagesRepository,
        IValidator<GroupPromotionImageCreateRequest> createRequestValidator,
        IValidator<GroupPromotionImageUpdateRequest> updateRequestValidator)
    {
        _groupPromotionImagesRepository = groupPromotionImagesRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    public Task<List<GroupPromotionImage>> GetAllInPromotionAsync(int groupPromotionId)
    {
        return _groupPromotionImagesRepository.GetAllInPromotionAsync(groupPromotionId);
    }

    public Task<List<GroupPromotionImageWithoutFile>> GetAllInPromotionWithoutFilesAsync(int groupPromotionId)
    {
        return _groupPromotionImagesRepository.GetAllInPromotionWithoutFilesAsync(groupPromotionId);
    }

    public Task<List<GroupPromotionImageWithoutFile>> GetAllWithoutFilesAsync()
    {
        return _groupPromotionImagesRepository.GetAllWithoutFilesAsync();
    }

    public Task<GroupPromotionImage?> GetByIdAsync(int id)
    {
        return _groupPromotionImagesRepository.GetByIdAsync(id);
    }

    public Task<List<GroupPromotionImage>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return _groupPromotionImagesRepository.GetByIdsAsync(ids);
    }

    public Task<List<GroupPromotionImageWithoutFile>> GetByIdsWithoutFilesAsync(IEnumerable<int> ids)
    {
        return _groupPromotionImagesRepository.GetByIdsWithoutFilesAsync(ids);
    }

    public Task<GroupPromotionImageWithoutFile?> GetByIdWithoutFileAsync(int id)
    {
        return _groupPromotionImagesRepository.GetByIdWithoutFileAsync(id);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<int, UnexpectedFailureResult> result = await _groupPromotionImagesRepository.InsertAsync(createRequest);

        return result.Map<int, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(GroupPromotionImageUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, NotFound> result = await _groupPromotionImagesRepository.UpdateAsync(updateRequest);

        return result.Map<Success, NotFound, ValidationResult>();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int id)
    {
        if (id <= 0) return new NotFound();

        return await _groupPromotionImagesRepository.DeleteAsync(id);
    }
}