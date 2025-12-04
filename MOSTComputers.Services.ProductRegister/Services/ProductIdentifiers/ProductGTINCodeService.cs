using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Utils.OneOf.MappingExtensions;

namespace MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers;
internal sealed class ProductGTINCodeService : IProductGTINCodeService
{
    public ProductGTINCodeService(
        IProductGTINCodeRepository productGTINCodeRepository,
        IProductRepository productRepository,
        IValidator<ServiceProductGTINCodeCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductGTINCodeUpdateRequest>? updateRequestValidator = null,
        IValidator<ServiceProductGTINCodeUpsertRequest>? upsertRequestValidator = null)
    {
        _productGTINCodeRepository = productGTINCodeRepository;
        _productRepository = productRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _upsertRequestValidator = upsertRequestValidator;
    }

    private readonly IProductGTINCodeRepository _productGTINCodeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<ServiceProductGTINCodeCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductGTINCodeUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ServiceProductGTINCodeUpsertRequest>? _upsertRequestValidator;

    public async Task<List<ProductGTINCode>> GetAllForProductAsync(int productId)
    {
        return await _productGTINCodeRepository.GetAllForProductAsync(productId);
    }

    public async Task<List<IGrouping<int, ProductGTINCode>>> GetAllForProductsAsync(List<int> productIds)
    {
        return await _productGTINCodeRepository.GetAllForProductsAsync(productIds);
    }

    public async Task<ProductGTINCode?> GetByProductIdAndTypeAsync(int productId, ProductGTINCodeType productGTINCodeType)
    {
        return await _productGTINCodeRepository.GetByProductIdAndTypeAsync(productId, productGTINCodeType);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductGTINCodeCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductGTINCode? existingCode = await GetByProductIdAndTypeAsync(createRequest.ProductId, createRequest.CodeType);

        if (existingCode is not null)
        {
            ValidationFailure codeAlreadyExistsError = new(nameof(ProductGTINCodeCreateRequest.CodeType),
                $"Product GTIN code of type {createRequest.CodeType} already exists for product with ID {createRequest.ProductId}.");

            return CreateValidationResultFromErrors(codeAlreadyExistsError);
        }

        ValidationResult productExistsValidationResult
            = await ValidateProductExistsAsync(createRequest.ProductId, nameof(ServiceProductGTINCodeCreateRequest.ProductId));

        if (!productExistsValidationResult.IsValid) return productExistsValidationResult;

        DateTime createDate = DateTime.Now;

        ProductGTINCodeCreateRequest innerCreateRequest = new()
        {
            ProductId = createRequest.ProductId,
            CodeType = createRequest.CodeType,
            CodeTypeAsText = createRequest.CodeTypeAsText,
            Value = createRequest.Value,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createDate
        };

        OneOf<Success, UnexpectedFailureResult> result = await _productGTINCodeRepository.InsertAsync(innerCreateRequest);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServiceProductGTINCodeUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        DateTime updateDate = DateTime.Now;

        ProductGTINCodeUpdateRequest innerUpdateRequest = new()
        {
            ProductId = updateRequest.ProductId,
            CodeType = updateRequest.CodeType,
            CodeTypeAsText = updateRequest.CodeTypeAsText,
            Value = updateRequest.Value,
            UpdateUserName = updateRequest.UpdateUserName,
            UpdateDate = updateDate,
        };

        OneOf<Success, NotFound> result = await _productGTINCodeRepository.UpdateAsync(innerUpdateRequest);

        return result.Map<Success, NotFound, ValidationResult>();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ServiceProductGTINCodeUpsertRequest upsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_upsertRequestValidator, upsertRequest);

        if (!validationResult.IsValid) return validationResult;

        ValidationResult productExistsValidationResult
            = await ValidateProductExistsAsync(upsertRequest.ProductId, nameof(ServiceProductGTINCodeUpsertRequest.ProductId));

        if (!productExistsValidationResult.IsValid) return productExistsValidationResult;

        DateTime upsertDate = DateTime.Now;

        ProductGTINCodeUpsertRequest innerUpsertRequest = new()
        {
            ProductId = upsertRequest.ProductId,
            CodeType = upsertRequest.CodeType,
            CodeTypeAsText = upsertRequest.CodeTypeAsText,
            Value = upsertRequest.Value,
            UpsertUserName = upsertRequest.UpsertUserName,
            UpsertDate = upsertDate,
        };

        OneOf<Success, UnexpectedFailureResult> result = await _productGTINCodeRepository.UpsertAsync(innerUpsertRequest);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int productId, ProductGTINCodeType productGTINCodeType)
    {
        return await _productGTINCodeRepository.DeleteAsync(productId, productGTINCodeType);
    }

    private async Task<ValidationResult> ValidateProductExistsAsync(int productId, string propertyName)
    {
        List<int> existingIds = await _productRepository.GetOnlyExistingIdsAsync([productId]);

        ValidationResult validationResult = new();

        if (existingIds.Count == 0)
        {
            ValidationFailure notFoundError = new(propertyName, $"Product with ID {productId} does not exist.");

            validationResult.Errors.Add(notFoundError);
        }

        return validationResult;
    }
}