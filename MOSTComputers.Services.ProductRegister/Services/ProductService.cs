using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(
        IProductRepository productRepository,
        IValidator<ProductCreateRequest>? createRequestValidator = null,
        IValidator<ProductUpdateRequest>? updateRequestValidator = null)
    {
        _productRepository = productRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productRepository.GetAll_WithManifacturerAndCategory();
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(ids);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest)
    {
        uint end = rangeSearchRequest.Start + rangeSearchRequest.Length;

        if (rangeSearchRequest.Start == end) return Enumerable.Empty<Product>();

        return _productRepository.GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(rangeSearchRequest.Start, end);
    }

    public Product? GetByIdWithFirstImage(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndFirstImage(id);
    }

    public Product? GetByIdWithProps(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndProperties(id);
    }

    public Product? GetByIdWithImages(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndImages(id);
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
        IValidator<ProductCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        if (createRequest.Images is not null)
        {
            foreach (var item in createRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<uint, UnexpectedFailureResult> result = _productRepository.Insert(createRequest);

        return result.Match<OneOf<uint, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        if (updateRequest.Images is not null)
        {
            foreach (var item in updateRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _productRepository.Delete(id);
    }
}