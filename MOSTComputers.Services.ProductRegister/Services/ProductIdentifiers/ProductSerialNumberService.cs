using OneOf;
using FluentValidation;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using FluentValidation.Results;
using MOSTComputers.Utils.OneOf;
using MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers;
internal sealed class ProductSerialNumberService : IProductSerialNumberService
{
    public ProductSerialNumberService(
        IProductSerialNumberRepository productSerialNumberRepository,
        IValidator<ProductSerialNumberCreateRequest>? createRequestValidator = null)
    {
        _productSerialNumberRepository = productSerialNumberRepository;
        _createRequestValidator = createRequestValidator;
    }

    private readonly IProductSerialNumberRepository _productSerialNumberRepository;
    private readonly IValidator<ProductSerialNumberCreateRequest>? _createRequestValidator;

    public async Task<List<IGrouping<int, ProductSerialNumber>>> GetAllForProductsAsync(List<int> productIds)
    {
        return await _productSerialNumberRepository.GetAllForProductsAsync(productIds);
    }

    public async Task<List<ProductSerialNumber>> GetAllForProductAsync(int productId)
    {
        return await _productSerialNumberRepository.GetAllForProductAsync(productId);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductSerialNumberCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        List<ProductSerialNumber> existingSerialNumbers = await GetAllForProductAsync(createRequest.ProductId);

        ProductSerialNumber? existingSerialNumber = existingSerialNumbers
            .FirstOrDefault(s => s.SerialNumber == createRequest.SerialNumber);

        if (existingSerialNumber is not null)
        {
            ValidationFailure serialNumberAlreadyExistsError = new(nameof(ProductSerialNumberCreateRequest.SerialNumber),
                $"Product serial number '{createRequest.SerialNumber}' already exists for product with ID {createRequest.ProductId}.");

            return CreateValidationResultFromErrors(serialNumberAlreadyExistsError);
        }

        OneOf<Success, UnexpectedFailureResult> result = await _productSerialNumberRepository.InsertAsync(createRequest);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int productId, string serialNumber)
    {
        return await _productSerialNumberRepository.DeleteAsync(productId, serialNumber);
    }
}