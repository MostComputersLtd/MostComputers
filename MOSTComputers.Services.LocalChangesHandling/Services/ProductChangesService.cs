using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.Product;

namespace MOSTComputers.Services.LocalChangesHandling.Services;

public sealed class ProductChangesService : IProductChangesService
{
    public ProductChangesService(
        ITransactionExecuteService transactionExecuteService,
        IProductService productService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductStatusesService productStatusesService,
        IProductDeserializeService productDeserializeService,
        ILocalChangesService localChangesService,
        IGetProductDataFromBeforeUpdateService getProductDataFromBeforeUpdateService)
    {
        _transactionExecuteService = transactionExecuteService;
        _productService = productService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productStatusesService = productStatusesService;
        _productDeserializeService = productDeserializeService;
        _localChangesService = localChangesService;
        _getProductDataFromBeforeUpdateService = getProductDataFromBeforeUpdateService;
    }

    private const string _tableChangeNameOfProductsTable = "MOSTPRices";

    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly ILocalChangesService _localChangesService;
    private readonly IGetProductDataFromBeforeUpdateService _getProductDataFromBeforeUpdateService;

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleAnyOperation(LocalChangeData localChangeData)
    {
        if (localChangeData.TableName != _tableChangeNameOfProductsTable)
        {
            List<ValidationFailure> failures = new()
            {
                new (nameof(localChangeData.TableName), "Argument must equal the name of the product table")
            };

            return new ValidationResult(failures);
        }

        if (localChangeData.OperationType == ChangeOperationTypeEnum.Create)
        {
            return HandleInsert(localChangeData);
        }
        else if (localChangeData.OperationType == ChangeOperationTypeEnum.Update)
        {
            return HandleUpdate(localChangeData);
        }
        else if (localChangeData.OperationType == ChangeOperationTypeEnum.Delete)
        {
            return HandleDelete(localChangeData);
        }

        return new UnexpectedFailureResult();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleInsert(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Create);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            OneOf<Success, UnexpectedFailureResult> handleInsertResult = _transactionExecuteService.ExecuteActionInTransaction(
                HandleInsertInternal,
                localChangeData);

            return handleInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                unexpectedFailureResult => unexpectedFailureResult);
        }
        catch (TransactionException)
        {
            return new UnexpectedFailureResult();
        }
        catch (ValidationException validationException)
        {
            return new ValidationResult(validationException.Errors);
        }
    }

    private OneOf<Success, UnexpectedFailureResult> HandleInsertInternal(LocalChangeData localChangeData)
    {
        int productId = localChangeData.TableElementId;

        Product? product = GetProductFull((uint)productId);

        if (product is null)
        {
            return new UnexpectedFailureResult();
        }

        ProductStatuses? productStatuses = _productStatusesService.GetByProductId((uint)productId);

        bool isProcessed = DetermineWhetherProductIsProcessedOrNot(product);

        if (productStatuses is not null)
        {
            ProductStatusesUpdateRequest productStatusesUpdateRequest = new()
            {
                ProductId = productId,
                IsProcessed = isProcessed,
                NeedsToBeUpdated = false,
            };

            OneOf<bool, ValidationResult> productStatusUpdateResult = _productStatusesService.Update(productStatusesUpdateRequest);

            return productStatusUpdateResult.Match<OneOf<Success, UnexpectedFailureResult>>(
                success => success ? new Success() : throw new TransactionInDoubtException("Operation failed for unknown reasons"),
                validationResult => throw new ValidationException(validationResult.Errors));
        }

        ProductStatusesCreateRequest productStatusesCreateRequest = new()
        {
            ProductId = productId,
            IsProcessed = isProcessed,
            NeedsToBeUpdated = false,
        };

        OneOf<Success, ValidationResult> productStatusInsertResult = _productStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

        return productStatusInsertResult.Match<OneOf<Success, UnexpectedFailureResult>>(
            success =>
            {
                _localChangesService.DeleteById((uint)localChangeData.Id);

                return success;
            },
            _ => throw new TransactionInDoubtException("Operation failed for unknown reasons"));
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleUpdate(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Update);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            OneOf<Success, UnexpectedFailureResult> handleUpdateResult = _transactionExecuteService.ExecuteActionInTransaction(
                HandleUpdateInternal,
                localChangeData);

            return handleUpdateResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                unexpectedFailureResult => unexpectedFailureResult);
        }
        catch (TransactionException)
        {
            return new UnexpectedFailureResult();
        }
        catch (ValidationException validationException)
        {
            return new ValidationResult(validationException.Errors);
        }
    }

    private OneOf<Success, UnexpectedFailureResult> HandleUpdateInternal(LocalChangeData localChangeData)
    {
        int productId = localChangeData.TableElementId;

        string productBeforeUpdateCacheKey = GetUpdatedByIdKey(productId);

        Product? productBeforeUpdate = _getProductDataFromBeforeUpdateService.GetProductBeforeUpdate((uint)productId);

        (Product? product,
        IEnumerable<ProductImage> productImages,
        IEnumerable<ProductImageFileNameInfo> productImageFileNames,
        IEnumerable<ProductProperty> productProperties)
            = GetProductInParts((uint)productId);

        if (product is null)
        {
            return new UnexpectedFailureResult();
        }

        product.Images = productImages
            .ToList();

        product.ImageFileNames = productImageFileNames
            .ToList();

        product.Properties = productProperties
            .ToList();

        bool isProcessed = DetermineWhetherProductIsProcessedOrNot(product);

        if (productBeforeUpdate != null)
        {
            bool needsToBeUpdated = DetermineWhetherUpdateIsImportant(productBeforeUpdate, product);

            if (needsToBeUpdated)
            {
                string xmlOfProduct = _productDeserializeService.SerializeProductsXml(product, true);

                foreach (ProductImage image in product.Images!)
                {
                    ServiceProductImageUpdateRequest productImageUpdateRequest = new()
                    {
                        Id = image.Id,
                        ImageData = image.ImageData,
                        ImageFileExtension = image.ImageFileExtension,
                        XML = xmlOfProduct,
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> imageUpdateResult = _productImageService.UpdateInAllImages(productImageUpdateRequest);

                    imageUpdateResult.Switch(
                        success => { },
                        validationResult => throw new ValidationException(validationResult.Errors),
                        unexpectedFailureResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"));
                }
            }

            ProductStatuses? productStatuses = _productStatusesService.GetByProductId((uint)productId);

            if (productStatuses is not null)
            {
                ProductStatusesUpdateRequest productStatusesUpdateRequest = new()
                {
                    ProductId = productId,
                    IsProcessed = isProcessed,
                    NeedsToBeUpdated = needsToBeUpdated,
                };

                OneOf<bool, ValidationResult> productStatusesUpdateResult = _productStatusesService.Update(productStatusesUpdateRequest);

                productStatusesUpdateResult.Switch(
                    success =>
                    {
                        if (!success)
                        {
                            throw new TransactionInDoubtException("Operation failed for unknown reasons");
                        }
                    },
                    validationResult => throw new ValidationException(validationResult.Errors));
            }
            else
            {
                ProductStatusesCreateRequest productStatusesCreateRequest = new()
                {
                    ProductId = productId,
                    IsProcessed = isProcessed,
                    NeedsToBeUpdated = needsToBeUpdated,
                };

                OneOf<Success, ValidationResult> productStatusesCreateResult = _productStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

                productStatusesCreateResult.Switch(
                    success => { },
                    validationResult => throw new ValidationException(validationResult.Errors));
            }
        }

        _getProductDataFromBeforeUpdateService.HandleAfterUpdate((uint)productId);

        _localChangesService.DeleteById((uint)localChangeData.Id);

        return new Success();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleDelete(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Delete);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            OneOf<Success, UnexpectedFailureResult> handleDeleteResult = _transactionExecuteService.ExecuteActionInTransaction(
                HandleDeleteInternal,
                localChangeData);

            return handleDeleteResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                unexpectedFailureResult => unexpectedFailureResult);
        }
        catch (TransactionException)
        {
            return new UnexpectedFailureResult();
        }
        catch (ValidationException validationException)
        {
            return new ValidationResult(validationException.Errors);
        }
    }

    private OneOf<Success, UnexpectedFailureResult> HandleDeleteInternal(LocalChangeData localChangeData)
    {
        uint productId = (uint)localChangeData.TableElementId;

        (Product? product,
        IEnumerable<ProductImage> productImages,
        IEnumerable<ProductImageFileNameInfo> productImageFileNames,
        IEnumerable<ProductProperty> productProperties)
            = GetProductInParts(productId);

        if (product is not null)
        {
            IEnumerable<LocalChangeData> changesForProductAfterDelete = GetLocalChangeDataAfterGivenTime(localChangeData.TimeStamp)
                .Where(localChangeDataLocal =>
                    localChangeDataLocal.TableName == _tableChangeNameOfProductsTable
                        && localChangeDataLocal.TableElementId == productId);

            if (changesForProductAfterDelete.Any())
            {
                LocalChangeData localChangeDataForProductFirst = changesForProductAfterDelete.First();

                if (localChangeDataForProductFirst.OperationType == ChangeOperationTypeEnum.Create)
                {
                    _localChangesService.DeleteById((uint)localChangeData.Id);

                    return new Success();
                }
            }

            throw new TransactionInDoubtException("Operation failed for unknown reasons");
        }

        ProductStatuses? productStatuses = _productStatusesService.GetByProductId(productId);

        if (productStatuses is not null)
        {
            bool isProductStatusDelete = _productStatusesService.DeleteByProductId(productId);

            if (!isProductStatusDelete)
            {
                throw new TransactionInDoubtException("Operation failed for unknown reasons");
            }
        }

        if (productImages.Any())
        { 
            bool areProductImagesDeleted = _productImageService.DeleteAllImagesForProduct(productId);

            if (!areProductImagesDeleted)
            {
                throw new TransactionInDoubtException("Operation failed for unknown reasons");
            }
        }

        if (productImageFileNames.Any())
        {
            bool areProductImageFileNameInfosDeleted = _productImageFileNameInfoService.DeleteAllForProductId(productId);

            if (!areProductImageFileNameInfosDeleted)
            {
                throw new TransactionInDoubtException("Operation failed for unknown reasons");
            }
        }

        if (productProperties.Any())
        {
            bool areProductPropertiesDeleted = _productPropertyService.DeleteAllForProduct(productId);

            if (!areProductPropertiesDeleted)
            {
                throw new TransactionInDoubtException("Operation failed for unknown reasons");
            }
        }

        _localChangesService.DeleteById((uint)localChangeData.Id);

        return new Success();
    }

    private static bool DetermineWhetherProductIsProcessedOrNot(Product productFull)
    {
        if (productFull is null)
        {
            return false;
        }

        if (productFull.Properties is not null
            && productFull.Properties.Count > 0)
        {
            return true;
        }

        if (productFull.Images is not null
            && productFull.Images.Count > 0)
        {
            return true;
        }

        return false;
    }

    private Product? GetProductFull(uint productId)
    {
        Product? product = _productService.GetByIdWithProps(productId);

        if (product is null) return null;

        if (product.Images is null
            || !product.Images.Any())
        {
            product.Images = _productImageService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || !product.ImageFileNames.Any())
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.Properties is null
            || !product.Properties.Any())
        {
            product.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        return product;
    }

    private (Product? product, IEnumerable<ProductImage> productImages,
        IEnumerable<ProductImageFileNameInfo> productImageFileNames, IEnumerable<ProductProperty> productProperties)
        GetProductInParts(uint productId)
    {
        Product? product = _productService.GetByIdWithProps(productId);

        IEnumerable<ProductImage>? productImages = null;
        IEnumerable<ProductImageFileNameInfo>? productImageFileNames = null;
        IEnumerable<ProductProperty>? productProperties = null;

        if (product is null
            || product.Images is null
            || !product.Images.Any())
        {
            productImages = _productImageService.GetAllInProduct(productId);
        }

        if (product is null
            || product.ImageFileNames is null
            || !product.ImageFileNames.Any())
        {
            productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);
        }

        if (product is null
            || product.Properties is null
            || !product.Properties.Any())
        {
            productProperties = _productPropertyService.GetAllInProduct(productId);
        }

        return (product, productImages ?? product!.Images!, productImageFileNames ?? product!.ImageFileNames!, productProperties ?? product!.Properties!);
    }

    private IEnumerable<LocalChangeData> GetLocalChangeDataAfterGivenTime(DateTime dateTime)
    {
        return _localChangesService.GetAll()
            .Where(localChangeData => localChangeData.TimeStamp > dateTime);
    }

    private bool DetermineWhetherUpdateIsImportant(Product productBeforeUpdate, Product productAfterUpdate)
    {
        if (productBeforeUpdate.SearchString != productAfterUpdate.SearchString
            || productBeforeUpdate.Status != productAfterUpdate.Status
            || productBeforeUpdate.CategoryID != productAfterUpdate.CategoryID
            || productBeforeUpdate.ManifacturerId != productAfterUpdate.ManifacturerId
            || productBeforeUpdate.Promotionid != productAfterUpdate.Promotionid
            || productBeforeUpdate.PromotionPictureId != productAfterUpdate.PromotionPictureId)
        {
            return true;
        }

        if (productBeforeUpdate.Properties is null
            || productBeforeUpdate.Properties.Count <= 0)
        {
            productBeforeUpdate.Properties = _productPropertyService.GetAllInProduct((uint)productBeforeUpdate.Id)
                .ToList();
        }

        if (productAfterUpdate.Properties is null
            || productAfterUpdate.Properties.Count <= 0)
        {
            productAfterUpdate.Properties = _productPropertyService.GetAllInProduct((uint)productAfterUpdate.Id)
                .ToList();
        }

        if (!CompareDataInPropertyCollections(productBeforeUpdate.Properties, productAfterUpdate.Properties))
        {
            return true;
        }

        return false;
    }

    private static bool CompareDataInPropertyCollections(List<ProductProperty> productProperties1, List<ProductProperty> productProperties2)
    {
        if (productProperties1.Count != productProperties2.Count)
        {
            return false;
        }

        productProperties1 = productProperties1
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        productProperties2 = productProperties2
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        for (int i = 0; i < productProperties1.Count; i++)
        {
            ProductProperty prop1 = productProperties1[i];
            ProductProperty prop2 = productProperties2[i];

            if (prop1.ProductCharacteristicId != prop2.ProductCharacteristicId
                || prop1.DisplayOrder != prop2.DisplayOrder
                || prop1.XmlPlacement != prop2.XmlPlacement) return false;
        }

        return true;
    }
}