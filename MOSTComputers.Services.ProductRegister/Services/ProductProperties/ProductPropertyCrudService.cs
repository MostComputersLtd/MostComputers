using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using System.Diagnostics.CodeAnalysis;

namespace MOSTComputers.Services.ProductRegister.Services.ProductProperties;
internal sealed class ProductPropertyCrudService : IProductPropertyCrudService
{
    public ProductPropertyCrudService(
        IProductRepository productRepository,
        IProductCharacteristicsRepository productCharacteristicsRepository,
        IProductPropertyRepository productPropertyRepository,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ServiceProductPropertyByCharacteristicIdCreateRequest>? createRequestByIdValidator = null,
        IValidator<ProductPropertyUpdateRequest>? updateRequestValidator = null)
    {
        _productRepository = productRepository;
        _productCharacteristicsRepository = productCharacteristicsRepository;
        _productPropertyRepository = productPropertyRepository;
        _transactionExecuteService = transactionExecuteService;
        _createRequestByIdValidator = createRequestByIdValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private const int _sharedProductCategoryId = -1;

    private readonly IProductRepository _productRepository;
    private readonly IProductCharacteristicsRepository _productCharacteristicsRepository;
    private readonly IProductPropertyRepository _productPropertyRepository;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly IValidator<ServiceProductPropertyByCharacteristicIdCreateRequest>? _createRequestByIdValidator;
    private readonly IValidator<ProductPropertyUpdateRequest>? _updateRequestValidator;

    public async Task<List<ProductProperty>> GetAllAsync()
    {
        return await _productPropertyRepository.GetAllAsync();
    }

    public async Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _productPropertyRepository.GetAllInProductsAsync(productIds);
    }

    public async Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _productPropertyRepository.GetCountOfAllInProductsAsync(productIds);
    }

    public async Task<List<ProductProperty>> GetAllInProductAsync(int productId)
    {
        if (productId <= 0) return new();

        return await _productPropertyRepository.GetAllInProductAsync(productId);
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        if (productId <= 0) return 0;

        return await _productPropertyRepository.GetCountOfAllInProductAsync(productId);
    }

    public async Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId)
    {
        if (productId <= 0) return null;

        return await _productPropertyRepository.GetByProductAndCharacteristicIdAsync(productId, characteristicId);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductPropertyByCharacteristicIdCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestByIdValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

        ValidationResult productValidationResult = CheckIfProductExistsAndHasValidCategory(product);

        if (!productValidationResult.IsValid) return productValidationResult;

        ProductCharacteristic? matchingCharacteristic = await _productCharacteristicsRepository.GetByIdAsync(createRequest.ProductCharacteristicId);

        ValidationResult characteristicValidationResult = CheckIfCorrectCharacteristicForPropertyExists(
            product!.CategoryId!.Value, matchingCharacteristic);

        if (!characteristicValidationResult.IsValid) return characteristicValidationResult;

        ProductPropertyCreateRequest createRequestInternal = new()
        {
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            ProductId = createRequest.ProductId,
            Name = matchingCharacteristic!.Name,
            DisplayOrder = createRequest.CustomDisplayOrder ?? matchingCharacteristic.DisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
        };

        return await _productPropertyRepository.InsertAsync(createRequestInternal);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> result = await _productPropertyRepository.UpdateAsync(updateRequest);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpdateRequest upsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, upsertRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(upsertRequest.ProductId);

        ValidationResult productValidationResult = CheckIfProductExistsAndHasValidCategory(product);

        if (!productValidationResult.IsValid) return productValidationResult;

        ProductCharacteristic? matchingCharacteristic = await _productCharacteristicsRepository.GetByIdAsync(upsertRequest.ProductCharacteristicId);

        ValidationResult characteristicValidationResult = CheckIfCorrectCharacteristicForPropertyExists(
            product!.CategoryId!.Value, matchingCharacteristic);

        if (!characteristicValidationResult.IsValid) return characteristicValidationResult;

        OneOf<Success, UnexpectedFailureResult> result = await UpsertInternalAsync(upsertRequest, matchingCharacteristic!);

        return result.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    private async Task<OneOf<Success, UnexpectedFailureResult>> UpsertInternalAsync(
        ProductPropertyUpdateRequest upsertRequest, ProductCharacteristic matchingCharacteristic)
    {
        ProductPropertyUpsertRequest upsertRequestInternal = new()
        {
            ProductCharacteristicId = upsertRequest.ProductCharacteristicId,
            ProductId = upsertRequest.ProductId,
            Name = matchingCharacteristic!.Name,
            DisplayOrder = upsertRequest.CustomDisplayOrder ?? matchingCharacteristic.DisplayOrder,
            Value = upsertRequest.Value,
            XmlPlacement = upsertRequest.XmlPlacement,
        };

        OneOf<Success, UnexpectedFailureResult> result = await _productPropertyRepository.UpsertAsync(upsertRequestInternal);

        return result;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdAsync(
        int productId, int oldCharacteristicId, int newCharacteristicId)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => ChangePropertyCharacteristicIdInternalAsync(productId, oldCharacteristicId, newCharacteristicId),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                notFound => false));
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdInternalAsync(
        int productId, int currentCharacteristicId, int newCharacteristicId)
    {
        ProductProperty? currentProperty = await GetByProductAndCharacteristicIdAsync(productId, currentCharacteristicId);

        if (currentProperty is null) return new NotFound();

        bool isCurrentPropertyDeleted = await DeleteAsync(productId, currentCharacteristicId);

        if (!isCurrentPropertyDeleted) return new NotFound();

        ServiceProductPropertyByCharacteristicIdCreateRequest propertyInsertRequest = new()
        {
            ProductId = productId,
            ProductCharacteristicId = newCharacteristicId,
            Value = currentProperty.Value,
            CustomDisplayOrder = currentProperty.DisplayOrder,
            XmlPlacement = currentProperty.XmlPlacement,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = await InsertAsync(propertyInsertRequest);

        return propertyInsertResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    }

    private static ValidationResult CheckIfProductExistsAndHasValidCategory(Product? product)
    {
        ValidationResult productExistsValidationResult = new();

        if (product is null)
        {
            productExistsValidationResult.Errors.Add(new(nameof(ServiceProductPropertyByCharacteristicNameCreateRequest.ProductId),
                "Id does not correspond to any known product"));

            return productExistsValidationResult;
        }

        if (product.CategoryId is null)
        {
            productExistsValidationResult.Errors.Add(new(nameof(ServiceProductPropertyByCharacteristicNameCreateRequest.ProductId),
                "Product is not a part of any known category"));

            return productExistsValidationResult;
        }

        return productExistsValidationResult;
    }

    private static ValidationResult CheckIfCorrectCharacteristicForPropertyExists(int productCategoryId, ProductCharacteristic? matchingCharacteristic)
    {
        ValidationResult noMatchingCharacteristicValidationResult = new();

        if (matchingCharacteristic is null)
        {
            ValidationFailure validationFailure = new(nameof(ServiceProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                "No characteristic was found with the given id");

            noMatchingCharacteristicValidationResult.Errors.Add(validationFailure);

            return noMatchingCharacteristicValidationResult;
        }

        if (matchingCharacteristic.CategoryId != productCategoryId
            && matchingCharacteristic.CategoryId != _sharedProductCategoryId)
        {
            ValidationFailure validationFailure = new(nameof(ServiceProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                "Characteristic is not from the correct category");

            noMatchingCharacteristicValidationResult.Errors.Add(validationFailure);

            return noMatchingCharacteristicValidationResult;
        }

        return noMatchingCharacteristicValidationResult;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(
        ProductPropertyUpsertAllForProductRequest changeAllForProductRequest)
    {
        Product? product = await _productRepository.GetByIdAsync(changeAllForProductRequest.ProductId);

        ValidationResult productValidationResult = CheckIfProductExistsAndHasValidCategory(product);

        int productCategoryId = product!.CategoryId!.Value;

        if (!productValidationResult.IsValid) return productValidationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertAllProductPropertiesInternalAsync(changeAllForProductRequest, productCategoryId),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesInternalAsync(
        ProductPropertyUpsertAllForProductRequest changeAllForProductRequest, int productCategoryId)
    {
        IEnumerable<int> neededCharacteristicIds = changeAllForProductRequest.NewProperties.Select(x => x.ProductCharacteristicId);

        List<ProductCharacteristic> neededCharacteristics = await _productCharacteristicsRepository.GetSelectionByCharacteristicIdsAsync(neededCharacteristicIds);

        List<ProductProperty> oldProperties = await GetAllInProductAsync(changeAllForProductRequest.ProductId);

        foreach (ProductProperty oldProperty in oldProperties)
        {
            if (oldProperty.ProductCharacteristicId is null) return new UnexpectedFailureResult();

            ProductPropertyForProductUpsertRequest? newProductPropForSameCharacteristic = changeAllForProductRequest.NewProperties
                .Find(newProperty => newProperty.ProductCharacteristicId == oldProperty.ProductCharacteristicId);

            if (newProductPropForSameCharacteristic is not null) continue;

            bool oldPropertyDeleteSuccess = await DeleteAsync(changeAllForProductRequest.ProductId, oldProperty.ProductCharacteristicId.Value);

            if (!oldPropertyDeleteSuccess) return new UnexpectedFailureResult();
        }

        Dictionary<ProductPropertyUpdateRequest, ProductCharacteristic> upsertData = new();

        foreach (ProductPropertyForProductUpsertRequest productPropUpsertRequest in changeAllForProductRequest.NewProperties)
        {
            ProductProperty? existingProductProperty = oldProperties.FirstOrDefault(x => x.ProductCharacteristicId == productPropUpsertRequest.ProductCharacteristicId);

            ProductCharacteristic productCharacteristic = neededCharacteristics.FirstOrDefault(x => x.Id == productPropUpsertRequest.ProductCharacteristicId)!;

            if (existingProductProperty is not null
                && existingProductProperty.DisplayOrder == (productPropUpsertRequest.CustomDisplayOrder ?? productCharacteristic.DisplayOrder)
                && existingProductProperty.Value == productPropUpsertRequest.Value
                && existingProductProperty.XmlPlacement == productPropUpsertRequest.XmlPlacement)
            {
                continue;
            }

            ValidationResult characteristicValidationResult = CheckIfCorrectCharacteristicForPropertyExists(
                productCategoryId, productCharacteristic);

            if (!characteristicValidationResult.IsValid) return characteristicValidationResult;

            ProductPropertyUpdateRequest propUpsertRequest = new()
            {
                ProductId = changeAllForProductRequest.ProductId,
                ProductCharacteristicId = productPropUpsertRequest.ProductCharacteristicId,
                CustomDisplayOrder = productPropUpsertRequest.CustomDisplayOrder,
                Value = productPropUpsertRequest.Value,
                XmlPlacement = productPropUpsertRequest.XmlPlacement,
            };

            ValidationResult validationResult = ValidateDefault(_updateRequestValidator, propUpsertRequest);

            if (!validationResult.IsValid) return validationResult;

            upsertData.Add(propUpsertRequest, productCharacteristic);
        }

        foreach (KeyValuePair<ProductPropertyUpdateRequest, ProductCharacteristic> kvp in upsertData)
        {
            ProductPropertyUpdateRequest propUpsertRequest = kvp.Key;
            ProductCharacteristic productCharacteristic = kvp.Value;

            OneOf<Success, UnexpectedFailureResult> propertyInsertResult = await UpsertInternalAsync(propUpsertRequest, productCharacteristic);

            bool isPropertyInsertSuccessful = propertyInsertResult.Match(
                success => true,
                unexpectedFailureResult => false);

            if (!isPropertyInsertSuccessful)
            {
                return propertyInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult>();
            }
        }

        return new Success();
    }

    public async Task<bool> DeleteAsync(int productId, int characteristicId)
    {
        if (productId <= 0 || characteristicId <= 0) return false;

        return await _productPropertyRepository.DeleteAsync(productId, characteristicId);
    }

    public async Task<bool> DeleteAllForProductAsync(int productId)
    {
        if (productId <= 0) return false;

        return await _productPropertyRepository.DeleteAllForProductAsync(productId);
    }

    public async Task<bool> DeleteAllForCharacteristicAsync(int characteristicId)
    {
        if (characteristicId <= 0) return false;

        return await _productPropertyRepository.DeleteAllForCharacteristicAsync(characteristicId);
    }
}