using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;

namespace MOSTComputers.Services.LocalChangesHandling.Services;

public sealed class ProductChangesService : IProductChangesService
{
    public ProductChangesService(
        ITransactionExecuteService transactionExecuteService,
        IProductService productService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductDeserializeService productDeserializeService,
        IProductHtmlService productHtmlService,
        ILocalChangesService localChangesService,
        IGetProductDataFromBeforeUpdateService getProductDataFromBeforeUpdateService,
        IToDoLocalChangesService toDoLocalChangesService)
    {
        _transactionExecuteService = transactionExecuteService;
        _productService = productService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productWorkStatusesService = productWorkStatusesService;
        _productDeserializeService = productDeserializeService;
        _productHtmlService = productHtmlService;
        _localChangesService = localChangesService;
        _getProductDataFromBeforeUpdateService = getProductDataFromBeforeUpdateService;
        _toDoLocalChangesService = toDoLocalChangesService;
    }

    private const string _tableChangeNameOfProductsTable = "MOSTPRices";

    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ILocalChangesService _localChangesService;
    private readonly IToDoLocalChangesService _toDoLocalChangesService;
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
            OneOf<Success, ValidationResult, UnexpectedFailureResult> handleInsertResult
                = _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                    () => HandleInsertInternal(localChangeData),
                    result => result.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false));

            return handleInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                validationResult => validationResult,
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

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleInsertInternal(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Create);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        int productId = localChangeData.TableElementId;

        Product? product = GetProductFull(productId);

        if (product is null)
        {
            return new UnexpectedFailureResult();
        }

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        ToDoLocalChangeCreateRequest toDoChangeCreateRequest = new()
        {
            TableName = localChangeData.TableName,
            OperationType = localChangeData.OperationType,
            TableElementId = localChangeData.TableElementId,
            TimeStamp = localChangeData.TimeStamp,
        };

        if (productWorkStatuses is not null)
        {
            OneOf<int, ValidationResult, UnexpectedFailureResult> insertToDoChangeResult
                = _toDoLocalChangesService.Insert(toDoChangeCreateRequest);

            return insertToDoChangeResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                id =>
                {
                    bool deleteChangeSuccess = _localChangesService.DeleteById(localChangeData.Id);

                    return deleteChangeSuccess ? new Success() : new UnexpectedFailureResult();
                },
                validationResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"),
                unexpectedFailureResult => unexpectedFailureResult);
        }

        ProductWorkStatusesCreateRequest productStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> productStatusInsertResult
            = _productWorkStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

        productStatusInsertResult.Switch(
            success => { },
            validationResult => throw new ValidationException(validationResult.Errors),
            unexpectedFailureResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"));

        OneOf<int, ValidationResult, UnexpectedFailureResult> newInsertToDoChangeResult
            = _toDoLocalChangesService.Insert(toDoChangeCreateRequest);

        return newInsertToDoChangeResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            id =>
            {
                bool deleteChangeSuccess = _localChangesService.DeleteById(localChangeData.Id);

                return deleteChangeSuccess ? new Success() : new UnexpectedFailureResult();
            },
            validationResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"),
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleUpdate(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Update);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            OneOf<Success, UnexpectedFailureResult> handleUpdateResult
                = _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                    () => HandleUpdateInternal(localChangeData),
                    result => result.Match(
                        success => true,
                        unexpectedFailureResult => false));

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

        string productBeforeUpdateCacheKey = CacheKeyUtils.ForProduct.GetUpdatedByIdKey(productId);

        Product? productBeforeUpdate = _getProductDataFromBeforeUpdateService.GetProductBeforeUpdate((uint)productId);

        (Product? product,
        IEnumerable<ProductImage> productImages,
        IEnumerable<ProductImageFileNameInfo> productImageFileNames,
        IEnumerable<ProductProperty> productProperties)
            = GetProductInParts(productId);

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
                string htmlOfProduct = _productHtmlService.GetHtmlFromProduct(product);

                OneOf<bool, ValidationResult, UnexpectedFailureResult> imagesHtmlUpdateResult
                    = _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductId(productId, htmlOfProduct);

                imagesHtmlUpdateResult.Switch(
                    isSuccessful =>
                    {
                        if (!isSuccessful)
                        {
                            throw new TransactionInDoubtException("Operation failed for unknown reasons");
                        }
                    },
                    validationResult => throw new ValidationException(validationResult.Errors),
                    unexpectedFailureResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"));
            }

            ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

            if (productWorkStatuses is not null)
            {
                ProductWorkStatusesUpdateByProductIdRequest productStatusesUpdateRequest = new()
                {
                    ProductId = productId,
                    ProductNewStatus = isProcessed ? ProductNewStatusEnum.WorkInProgress : ProductNewStatusEnum.New,
                    ProductXmlStatus = productWorkStatuses.ProductXmlStatus,
                    ReadyForImageInsert = productWorkStatuses.ReadyForImageInsert,
                };

                OneOf<bool, ValidationResult> productStatusesUpdateResult = _productWorkStatusesService.UpdateByProductId(productStatusesUpdateRequest);

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
                ProductWorkStatusesCreateRequest productStatusesCreateRequest = new()
                {
                    ProductId = productId,
                    ProductNewStatus = isProcessed ? ProductNewStatusEnum.WorkInProgress : ProductNewStatusEnum.New,
                    ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                    ReadyForImageInsert = false,
                };

                OneOf<int, ValidationResult, UnexpectedFailureResult> productStatusesCreateResult
                    = _productWorkStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

                productStatusesCreateResult.Switch(
                    success => { },
                    validationResult => throw new ValidationException(validationResult.Errors),
                    unexpectedFailureResult => throw new TransactionInDoubtException("Operation failed for unknown reasons"));
            }
        }

        _getProductDataFromBeforeUpdateService.HandleAfterUpdate((uint)productId);

        bool localChangeDeleteResult = _localChangesService.DeleteById(localChangeData.Id);

        return localChangeDeleteResult ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleDelete(LocalChangeData localChangeData)
    {
        LocalChangeDataValidatorForGivenTableAndOperationType validator = new(_tableChangeNameOfProductsTable, ChangeOperationTypeEnum.Delete);

        ValidationResult validationResult = validator.Validate(localChangeData);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            OneOf<Success, UnexpectedFailureResult> handleDeleteResult
               = _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                   () => HandleDeleteInternal(localChangeData),
                   result => result.Match(
                       success => true,
                       unexpectedFailureResult => false));

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
        int productId = localChangeData.TableElementId;

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
                    _localChangesService.DeleteById(localChangeData.Id);

                    return new Success();
                }
            }

            throw new TransactionInDoubtException("Operation failed for unknown reasons");
        }

        ProductWorkStatuses? productStatuses = _productWorkStatusesService.GetByProductId(productId);

        if (productStatuses is not null)
        {
            bool isProductStatusDelete = _productWorkStatusesService.DeleteByProductId(productId);

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

        _localChangesService.DeleteById(localChangeData.Id);

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

    private Product? GetProductFull(int productId)
    {
        Product? product = _productService.GetByIdWithProps(productId);

        if (product is null) return null;

        if (product.Images is null
            || product.Images.Count == 0)
        {
            product.Images = _productImageService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count == 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.Properties is null
            || product.Properties.Count == 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        return product;
    }

    private (Product? product, IEnumerable<ProductImage> productImages,
        IEnumerable<ProductImageFileNameInfo> productImageFileNames, IEnumerable<ProductProperty> productProperties)
        GetProductInParts(int productId)
    {
        Product? product = _productService.GetByIdWithProps(productId);

        IEnumerable<ProductImage>? productImages = null;
        IEnumerable<ProductImageFileNameInfo>? productImageFileNames = null;
        IEnumerable<ProductProperty>? productProperties = null;

        if (product is null
            || product.Images is null
            || product.Images.Count == 0)
        {
            productImages = _productImageService.GetAllInProduct(productId);
        }

        if (product is null
            || product.ImageFileNames is null
            || product.ImageFileNames.Count == 0)
        {
            productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);
        }

        if (product is null
            || product.Properties is null
            || product.Properties.Count == 0)
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
            || productBeforeUpdate.CategoryId != productAfterUpdate.CategoryId
            || productBeforeUpdate.ManifacturerId != productAfterUpdate.ManifacturerId
            || productBeforeUpdate.PromotionId != productAfterUpdate.PromotionId
            || productBeforeUpdate.PromotionPictureId != productAfterUpdate.PromotionPictureId)
        {
            return true;
        }

        if (productBeforeUpdate.Properties is null
            || productBeforeUpdate.Properties.Count <= 0)
        {
            productBeforeUpdate.Properties = _productPropertyService.GetAllInProduct(productBeforeUpdate.Id)
                .ToList();
        }

        if (productAfterUpdate.Properties is null
            || productAfterUpdate.Properties.Count <= 0)
        {
            productAfterUpdate.Properties = _productPropertyService.GetAllInProduct(productAfterUpdate.Id)
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