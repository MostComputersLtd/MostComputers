using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Mapping;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class CategoryService : ICategoryService
{
    public CategoryService(
        ICategoryRepository categoryRepository,
        ProductMapper productMapper,
        IValidator<ServiceCategoryCreateRequest>? createRequestValidator = null,
        IValidator<ServiceCategoryUpdateRequest>? updateRequestValidator = null)
    {
        _categoryRepository = categoryRepository;
        _productMapper = productMapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly ICategoryRepository _categoryRepository;
    private readonly ProductMapper _productMapper;
    private readonly IValidator<ServiceCategoryCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceCategoryUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Category> GetAll()
    {
        return _categoryRepository.GetAll();
    }

    public Category? GetById(uint id)
    {
        return _categoryRepository.GetById(id);
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ServiceCategoryCreateRequest createRequest,
        IValidator<ServiceCategoryCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        CategoryCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.RowGuid = Guid.NewGuid();
        createRequestInternal.IsLeaf = (createRequestInternal.ParentCategoryId is not null);

        OneOf<uint, UnexpectedFailureResult> result = _categoryRepository.Insert(createRequestInternal);

        return result.Match<OneOf<uint, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceCategoryUpdateRequest updateRequest,
        IValidator<ServiceCategoryUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        CategoryUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.RowGuid = Guid.NewGuid();

        OneOf<Success, UnexpectedFailureResult> result = _categoryRepository.Update(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _categoryRepository.Delete(id);
    }
}