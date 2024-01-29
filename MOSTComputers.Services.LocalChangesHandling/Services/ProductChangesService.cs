using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
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
        ICache<string> cache)
    {
        _transactionExecuteService = transactionExecuteService;
        _productService = productService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productStatusesService = productStatusesService;
        _productDeserializeService = productDeserializeService;
        _localChangesService = localChangesService;
        _cache = cache;
    }

    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly ILocalChangesService _localChangesService;
    private readonly ICache<string> _cache;

    public OneOf<Success, UnexpectedFailureResult> HandleInsert(LocalChangeData localChangeData)
    {
        if (localChangeData.TableElementId < 0) return new UnexpectedFailureResult();

        return _transactionExecuteService.ExecuteActionInTransaction(
            HandleInsertInternal,
            localChangeData);
    }

    private OneOf<Success, UnexpectedFailureResult> HandleInsertInternal(LocalChangeData localChangeData)
    {
        int productId = localChangeData.TableElementId;

        if (productId < 0) return new UnexpectedFailureResult();

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

    public OneOf<Success, UnexpectedFailureResult> HandleUpdate(LocalChangeData localChangeData)
    {
        if (localChangeData.TableElementId < 0) return new UnexpectedFailureResult();

        return _transactionExecuteService.ExecuteActionInTransaction(
            HandleUpdateInternal,
            localChangeData);
    }

    private OneOf<Success, UnexpectedFailureResult> HandleUpdateInternal(LocalChangeData localChangeData)
    {
        int productId = localChangeData.TableElementId;

        if (productId < 0) return new UnexpectedFailureResult();

        string productBeforeUpdateCacheKey = GetUpdatedByIdKey(productId);

        Product? productBeforeUpdate = _cache.GetValueOrDefault<Product>(productBeforeUpdateCacheKey);

        Product? product = GetProductFull((uint)productId);

        if (product is null)
        {
            return new UnexpectedFailureResult();
        }

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
                        _ => { },
                        validationResult => throw new ValidationException(validationResult.Errors),
                        _ => throw new TransactionInDoubtException("Operation failed for unknown reasons"));
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
        }

        _cache.Evict(productBeforeUpdateCacheKey);

        _localChangesService.DeleteById((uint)localChangeData.Id);

        return new Success();
    }

    public OneOf<Success, UnexpectedFailureResult> HandleDelete(LocalChangeData localChangeData)
    {
        int productId = localChangeData.TableElementId;

        if (productId < 0) return new UnexpectedFailureResult();

        bool success = _productStatusesService.DeleteByProductId((uint)productId);

        return success ? new Success() : new UnexpectedFailureResult();
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
            product.ImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId)
                .ToList();
        }

        return product;
    }

    private bool DetermineWhetherUpdateIsImportant(Product productBeforeUpdate, Product product)
    {
        if (productBeforeUpdate.SearchString != product.SearchString
            || productBeforeUpdate.Status != product.Status
            || productBeforeUpdate.CategoryID != product.CategoryID
            || productBeforeUpdate.ManifacturerId != product.ManifacturerId
            || productBeforeUpdate.Promotionid != product.Promotionid
            || productBeforeUpdate.PromotionPictureId != product.PromotionPictureId)
        {
            return true;
        }

        if (productBeforeUpdate.Properties is null
            || productBeforeUpdate.Properties.Count <= 0)
        {
            productBeforeUpdate.Properties = _productPropertyService.GetAllInProduct((uint)productBeforeUpdate.Id)
                .ToList();
        }

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct((uint)product.Id)
                .ToList();
        }

        if (!CompareDataInPropertyCollections(productBeforeUpdate.Properties, product.Properties))
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