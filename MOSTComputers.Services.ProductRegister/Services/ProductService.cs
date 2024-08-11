using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(
        IProductRepository productRepository,
        IProductPropertyService productPropertyService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IValidator<ProductCreateRequest>? createRequestValidator = null,
        IValidator<ProductUpdateRequest>? updateRequestValidator = null)
    {
        _productRepository = productRepository;
        _productPropertyService = productPropertyService;
        _productImageFileNameInfoService = productImageFileNameInfoService;

        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductRepository _productRepository;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IValidator<ProductCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productRepository.GetAll_WithManifacturerAndCategory();
    }

    public IEnumerable<Product> GetAllWhereSearchStringMatches(string searchStringParts)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(searchStringParts);
    }

    public IEnumerable<Product> GetAllWhereNameMatches(string subString)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_WhereSearchNameContainsSubstring(subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereSearchStringMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereNameContainsSubstring(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategoryAndStatuses_WhereAllConditionsAreMet(productRangeSearchRequest.Start, end, productConditionalSearchRequest);
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _productRepository.GetAll_WithManifacturerAndCategory_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _productRepository.GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(ids);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest)
    {
        if (rangeSearchRequest.Start <= 0
            || rangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        uint end = (uint)rangeSearchRequest.Start + rangeSearchRequest.Length;

        if (rangeSearchRequest.Start == end) return Enumerable.Empty<Product>();

        return _productRepository.GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(rangeSearchRequest.Start, end);
    }

    public Product? GetByIdWithFirstImage(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndFirstImage(id);
    }

    public Product? GetByIdWithProps(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndProperties(id);
    }

    public Product? GetByIdWithImages(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndImages(id);
    }

    public Product? GetProductWithHighestId()
    {
        return _productRepository.GetProductWithHighestId_WithManifacturerAndCategory();
    }

    public Product? GetProductFull(int productId)
    {
        if (productId <= 0) return null;

        Product? product = _productRepository.GetById_WithManifacturerAndCategoryAndImages(productId);

        if (product is null) return null;

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        return product;
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
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

        OneOf<int, UnexpectedFailureResult> result = _productRepository.Insert(createRequest);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        if (updateRequest.Images is not null)
        {
            foreach (CurrentProductImageUpdateRequest item in updateRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        return _productRepository.Delete(id);
    }
}