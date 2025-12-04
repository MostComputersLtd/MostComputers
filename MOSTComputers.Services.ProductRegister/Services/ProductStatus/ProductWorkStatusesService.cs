using FluentValidation;
using FluentValidation.Results;
using OneOf;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus;
internal class ProductWorkStatusesService : IProductWorkStatusesService
{
    public ProductWorkStatusesService(
        IProductRepository productRepository,
        IProductWorkStatusesRepository productWorkStatusesRepository,
        IValidator<ServiceProductWorkStatusesCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductWorkStatusesUpdateByIdRequest>? updateByIdRequestValidator = null,
        IValidator<ServiceProductWorkStatusesUpdateByProductIdRequest>? updateByProductIdRequestValidator = null,
        IValidator<ServiceProductWorkStatusesUpsertRequest>? upsertRequestValidator = null)
    {
        _productRepository = productRepository;
        _productWorkStatusesRepository = productWorkStatusesRepository;
        _createRequestValidator = createRequestValidator;
        _updateByIdRequestValidator = updateByIdRequestValidator;
        _updateByProductIdRequestValidator = updateByProductIdRequestValidator;
        _upsertRequestValidator = upsertRequestValidator;
    }

    private readonly IProductRepository _productRepository;
    private readonly IProductWorkStatusesRepository _productWorkStatusesRepository;

    private readonly IValidator<ServiceProductWorkStatusesCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductWorkStatusesUpdateByIdRequest>? _updateByIdRequestValidator;
    private readonly IValidator<ServiceProductWorkStatusesUpdateByProductIdRequest>? _updateByProductIdRequestValidator;
    private readonly IValidator<ServiceProductWorkStatusesUpsertRequest>? _upsertRequestValidator;

    public async Task<List<ProductWorkStatuses>> GetAllAsync()
    {
        return await _productWorkStatusesRepository.GetAllAsync();
    }

    public async Task<List<ProductWorkStatuses>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _productWorkStatusesRepository.GetAllForProductsAsync(productIds);
    }

    public async Task<ProductWorkStatuses?> GetByIdAsync(int productWorkStatusesId)
    {
        return await _productWorkStatusesRepository.GetByIdAsync(productWorkStatusesId);
    }

    public async Task<ProductWorkStatuses?> GetByProductIdAsync(int productId)
    {
        return await _productWorkStatusesRepository.GetByProductIdAsync(productId);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertIfItDoesntExistAsync(ServiceProductWorkStatusesCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

        if (product is null)
        {
            ValidationResult productExistsValidationResult = new();

            productExistsValidationResult.Errors.Add(new(nameof(ServiceProductPropertyByCharacteristicNameCreateRequest.ProductId),
                "Id does not correspond to any known product"));

            return productExistsValidationResult;
        }

        DateTime createDate = DateTime.Now;

        ProductWorkStatusesCreateRequest createRequestInner = new()
        {
            ProductId = createRequest.ProductId,
            ProductNewStatus = createRequest.ProductNewStatus,
            ProductXmlStatus = createRequest.ProductXmlStatus,
            ReadyForImageInsert = createRequest.ReadyForImageInsert,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createDate,
            LastUpdateUserName = createRequest.CreateUserName,
            LastUpdateDate = createDate,
        };

        return await _productWorkStatusesRepository.InsertIfItDoesntExistAsync(createRequestInner);
    }

    public async Task<OneOf<bool, ValidationResult>> UpdateByIdAsync(ServiceProductWorkStatusesUpdateByIdRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateByIdRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        DateTime updateDate = DateTime.Now;

        ProductWorkStatusesUpdateByIdRequest updateRequestInner = new()
        {
            Id = updateRequest.Id,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
            LastUpdateUserName = updateRequest.LastUpdateUserName,
            LastUpdateDate = updateDate,
        };

        return await _productWorkStatusesRepository.UpdateByIdAsync(updateRequestInner);
    }

    public async Task<OneOf<bool, ValidationResult>> UpdateByProductIdAsync(ServiceProductWorkStatusesUpdateByProductIdRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateByProductIdRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        DateTime updateDate = DateTime.Now;

        ProductWorkStatusesUpdateByProductIdRequest updateRequestInner = new()
        {
            ProductId = updateRequest.ProductId,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
            LastUpdateUserName = updateRequest.LastUpdateUserName,
            LastUpdateDate = updateDate,
        };

        return await _productWorkStatusesRepository.UpdateByProductIdAsync(updateRequestInner);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertByProductIdAsync(ServiceProductWorkStatusesUpsertRequest upsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_upsertRequestValidator, upsertRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(upsertRequest.ProductId);

        if (product is null)
        {
            ValidationResult productExistsValidationResult = new();

            productExistsValidationResult.Errors.Add(new(nameof(ServiceProductPropertyByCharacteristicNameCreateRequest.ProductId),
                "Id does not correspond to any known product"));

            return productExistsValidationResult;
        }

        DateTime upsertDate = DateTime.Now;

        ProductWorkStatusesUpsertRequest updateRequestInner = new()
        {
            ProductId = upsertRequest.ProductId,
            ProductNewStatus = upsertRequest.ProductNewStatus,
            ProductXmlStatus = upsertRequest.ProductXmlStatus,
            ReadyForImageInsert = upsertRequest.ReadyForImageInsert,
            UpsertUserName = upsertRequest.UpsertUserName,
            UpsertDate = upsertDate,
        };

        OneOf<int, UnexpectedFailureResult> upsertResult = await _productWorkStatusesRepository.UpsertByProductIdAsync(updateRequestInner);

        return upsertResult.Map<int, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<bool> DeleteAllAsync()
    {
        return await _productWorkStatusesRepository.DeleteAllAsync();
    }

    public async Task<bool> DeleteByProductIdAsync(int productId)
    {
        return await _productWorkStatusesRepository.DeleteByProductIdAsync(productId);
    }

    public async Task<bool> DeleteByIdAsync(int productWorkStatusesId)
    {
        return await _productWorkStatusesRepository.DeleteByIdAsync(productWorkStatusesId);
    }
}