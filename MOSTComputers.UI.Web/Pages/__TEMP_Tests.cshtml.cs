using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using System.Reflection;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;

using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;
using SixLabors.ImageSharp;
using System.Diagnostics;
using MOSTComputers.Utils.OneOf;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;

namespace MOSTComputers.UI.Web.Pages;

[Authorize(Roles = "Admin")]
public class TEMP_TestsModel : PageModel
{
    public TEMP_TestsModel(
        IImageExtensionFromContentTypeService fileExtensionFromContentTypeService,
        IProductService productService,
        IProductImageService productImageService,
        IProductCharacteristicService productCharacteristicService,
        IProductPropertyService productPropertyService,
        IProductCharacteristicAndExternalXmlDataRelationService productCharacteristicAndExternalXmlDataRelationService,
        ILegacyProductHtmlService legacyProductHtmlService,
        ITransactionExecuteService transactionExecuteService)
    {
        _fileExtensionFromContentTypeService = fileExtensionFromContentTypeService;
        _productService = productService;
        _productImageService = productImageService;
        _productCharacteristicService = productCharacteristicService;
        _productPropertyService = productPropertyService;
        _productCharacteristicAndExternalXmlDataRelationService = productCharacteristicAndExternalXmlDataRelationService;
        _legacyProductHtmlService = legacyProductHtmlService;
        _transactionExecuteService = transactionExecuteService;
    }

    private const int _defaultLinkCharacteristicId = 1693;

    private readonly IImageExtensionFromContentTypeService _fileExtensionFromContentTypeService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicAndExternalXmlDataRelationService _productCharacteristicAndExternalXmlDataRelationService;
    private readonly ILegacyProductHtmlService _legacyProductHtmlService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private static void TEMP__AllowTransactionScopesWithLongTimeouts(TimeSpan maxTimeout)
    {
        TEMP__SetTransactionManagerField("s_cachedMaxTimeout", true);
        TEMP__SetTransactionManagerField("s_maximumTimeout", maxTimeout);

        static void TEMP__SetTransactionManagerField(string fieldName, object value)
        {
            typeof(TransactionManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, value);
        }
    }

    public async Task<IActionResult> OnGetSaveAllImagesAndFilesAndPropertiesFromHtmlAsync()
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        List<Product> products = await _productService.GetAllAsync();

        ProductTestMetrics productTestMetrics = new();

        TimeSpan timeout = TimeSpan.FromDays(2);

        TEMP__AllowTransactionScopesWithLongTimeouts(timeout);

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result =
            await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                () => SaveAllImagesAndFilesAndAllPropertiesFromHtmlInternalAsync(currentUserName, products, productTestMetrics),
                result => result.IsT0,
                transactionOptions: new()
                {
                    Timeout = timeout
                });

        return result.Match(
            success => new JsonResult(new { metrics = productTestMetrics }),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnGetSaveUnsavedImagesAndFilesAndPropertiesFromHtmlAsync()
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        List<Product> products = await _productService.GetAllAsync();

        ProductTestMetrics productTestMetrics = new();

        TimeSpan timeout = TimeSpan.FromDays(2);

        TEMP__AllowTransactionScopesWithLongTimeouts(timeout);

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result =
            await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                () => SaveUnsavedImagesAndFilesAndAllPropertiesFromHtmlInternalAsync(currentUserName, products, productTestMetrics),
                result => result.IsT0,
                transactionOptions: new()
                {
                    Timeout = timeout
                });

        return result.Match(
            success => new JsonResult(new { metrics = productTestMetrics }),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(fileSaveFailureResult),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveAllImagesAndFilesAndAllPropertiesFromHtmlInternalAsync(
            string currentUserName, List<Product> products, ProductTestMetrics? productTestMetrics = null)
    {
        const int productsProcessingAtTheSameTimeBatchSize = 1000;

        for (int i = 0; i < products.Count; i += productsProcessingAtTheSameTimeBatchSize)
        {
            int batchCount = Math.Min(productsProcessingAtTheSameTimeBatchSize, products.Count - i);

            List<Product> currentProductBatch = products.GetRange(i, batchCount);

            List<IGrouping<Product?, ProductImage>> originalAllOrFirstImagesForBatch = await GetAllOrFirstOriginalImagesForProductsWithDataAsync(currentProductBatch);

            List<IGrouping<int?, ProductImage>> originalImagesForBatch = originalAllOrFirstImagesForBatch
                .SelectMany(x => x)
                .GroupBy(x => x.ProductId)
                .ToList();

            List<IGrouping<Product?, ProductImageData>> originalImagesDataForBatch = originalAllOrFirstImagesForBatch
                .Select(group =>
                    group.Select(image => new ProductImageData
                    {
                        Id = image.Id,
                        ProductId = image.ProductId,
                        HtmlData = image.HtmlData,
                        ImageContentType = image.ImageContentType,
                        DateModified = image.DateModified
                    })
                    .GroupBy(_ => group.Key)
                    .First()
                )
                .ToList();

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> saveBatchResult
                = await SaveImagesAndFilesForProductsInternalAsync(originalImagesForBatch, currentUserName, productTestMetrics);

            if (!saveBatchResult.IsT0)
            {
                return saveBatchResult;
            }

            OneOf<Success, ValidationResult, UnexpectedFailureResult> savePropertiesResult
                = await SaveProductPropertiesFromOriginalImageHtmlAsync(
                    originalImagesDataForBatch, currentUserName, productTestMetrics);

            if (!savePropertiesResult.IsT0)
            {
                return savePropertiesResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveUnsavedImagesAndFilesAndAllPropertiesFromHtmlInternalAsync(
            string currentUserName, List<Product> products, ProductTestMetrics? productTestMetrics = null)
    {
        const int productsProcessingAtTheSameTimeBatchSize = 1000;

        for (int i = 0; i < products.Count; i += productsProcessingAtTheSameTimeBatchSize)
        {
            int batchCount = Math.Min(productsProcessingAtTheSameTimeBatchSize, products.Count - i);

            List<Product> currentProductBatch = products.GetRange(i, batchCount);

            List<IGrouping<Product?, ProductImage>> originalAllOrFirstImagesForBatch = await GetAllOrFirstOriginalImagesForProductsWithDataAsync(currentProductBatch);

            List<IGrouping<int?, ProductImage>> originalImagesForBatch = originalAllOrFirstImagesForBatch
                .SelectMany(x => x)
                .GroupBy(x => x.ProductId)
                .ToList();

            List<IGrouping<Product?, ProductImageData>> originalImagesDataForBatch = originalAllOrFirstImagesForBatch
                .Select(group =>
                    group.Select(image => new ProductImageData
                    {
                        Id = image.Id,
                        ProductId = image.ProductId,
                        HtmlData = image.HtmlData,
                        ImageContentType = image.ImageContentType,
                        DateModified = image.DateModified
                    })
                    .GroupBy(_ => group.Key)
                    .First()
                )
                .ToList();

            IEnumerable<int> currentProductBatchIds = currentProductBatch.Select(product => product.Id);

            List<IGrouping<int, ProductImage>> localProductImages = await _productImageService.GetAllInProductsAsync(currentProductBatchIds);

            List<IGrouping<int, ProductProperty>> propertiesOfProducts = await _productPropertyService.GetAllInProductsAsync(currentProductBatchIds);

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> saveBatchResult
                = await SaveUnsavedImagesAndFilesForProductsInternalAsync(originalImagesForBatch, localProductImages, currentUserName, productTestMetrics);

            if (!saveBatchResult.IsT0) return saveBatchResult;

            OneOf<Success, ValidationResult, UnexpectedFailureResult> saveUnsavedPropertiesResult = await SaveUnsavedProductPropertiesFromOriginalImageHtmlInternalAsync(
                originalImagesDataForBatch, propertiesOfProducts, currentUserName, productTestMetrics);

            if (!saveUnsavedPropertiesResult.IsT0)
            {
                return saveUnsavedPropertiesResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
            }
        }

        return new Success();
    }

    public async Task<IActionResult> OnGetSaveAllImageFilesAsync()
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        List<Product> products = await _productService.GetAllAsync();

        ProductTestMetrics productTestMetrics = new();

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> saveAllImagesResult
            = await SaveProductImagesToDirectoryAsync(products, currentUserName, productTestMetrics);

        return saveAllImagesResult.Match(
            success => new JsonResult(new { metrics = productTestMetrics }),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> SaveProductImagesToDirectoryAsync(
        List<Product> products, string upsertUserName, ProductTestMetrics? productTestMetrics = null)
    {
        TimeSpan timeout = TimeSpan.FromDays(2);

        TEMP__AllowTransactionScopesWithLongTimeouts(timeout);

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => SaveImagesAndFilesForProductsInternalAsync(products, upsertUserName, productTestMetrics),
            result => result.Match(
                success => true,
                validationResult => false,
                fileSaveFailureResult => false,
                fileDoesntExistResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false),
            transactionOptions: new()
            {
                Timeout = timeout
            });
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveImagesAndFilesForProductsInternalAsync(List<Product> products, string upsertUserName, ProductTestMetrics? productTestMetrics = null)
    {
        const int productsProcessingAtTheSameTimeBatchSize = 1000;

        for (int i = 0; i < products.Count; i += productsProcessingAtTheSameTimeBatchSize)
        {
            int batchCount = Math.Min(productsProcessingAtTheSameTimeBatchSize, products.Count - i);

            List<Product> currentProductBatch = products.GetRange(i, batchCount);

            List<IGrouping<Product?, ProductImage>> originalAllOrFirstImagesForBatch = await GetAllOrFirstOriginalImagesForProductsWithDataAsync(currentProductBatch);

            List<IGrouping<int?, ProductImage>> allOriginalProductImages = originalAllOrFirstImagesForBatch
                .SelectMany(x => x)
                .GroupBy(x => x.ProductId)
                .ToList();

            List<ProductImageWithFileUpsertRequest> imageUpsertRequests = new();

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> saveBatchResult
                = await SaveImagesAndFilesForProductsInternalAsync(allOriginalProductImages, upsertUserName, productTestMetrics);

            if (!saveBatchResult.IsT0) return saveBatchResult;
        }

        return new Success();
    }

    
    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveImagesAndFilesForProductsInternalAsync(
            List<IGrouping<int?, ProductImage>> originalProductImages,
            string upsertUserName,
            ProductTestMetrics? productTestMetrics = null)
    {
        foreach (IGrouping<int?, ProductImage> originalImagesByProduct in originalProductImages)
        {
            if (originalImagesByProduct.Key is null) continue;

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertAllResult
                 = await SaveImagesAndFilesForProductAsync(upsertUserName, originalImagesByProduct, productTestMetrics);

            if (!upsertAllResult.IsT0) return upsertAllResult;
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveUnsavedImagesAndFilesForProductsInternalAsync(
            List<IGrouping<int?, ProductImage>> originalProductImages,
            List<IGrouping<int, ProductImage>> localProductImages,
            string upsertUserName,
            ProductTestMetrics? productTestMetrics = null)
    {
        foreach (IGrouping<int?, ProductImage> originalImagesByProduct in originalProductImages)
        {
            if (originalImagesByProduct.Key is null) continue;

            IGrouping<int, ProductImage>? existingProductImages = localProductImages
                .FirstOrDefault(group => group.Key == originalImagesByProduct.Key.Value);

            if (originalImagesByProduct.Count() == originalImagesByProduct.Count()) continue;

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertAllResult
                 = await SaveImagesAndFilesForProductAsync(upsertUserName, originalImagesByProduct, productTestMetrics);

            if (!upsertAllResult.IsT0) return upsertAllResult;
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult ,FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> SaveImagesAndFilesForProductAsync(
        string upsertUserName,
        IGrouping<int?, ProductImage> originalImagesByProduct,
        ProductTestMetrics? productTestMetrics = null)
    {
        List<ProductImageWithFileForProductUpsertRequest> productImageWithFileUpsertRequests = new();

        foreach (ProductImage originalImage in originalImagesByProduct)
        {
            if (originalImage.ImageData is null
                || originalImage.ImageData.Length <= 0
                || originalImage.ImageContentType is null
                || originalImage.ImageContentType == "-?-")
            {
                continue;
            }

            MemoryStream imageDataStream = new(originalImage.ImageData);

            if (!IsImageDataValid(imageDataStream))
            {
                continue;
            }

            //if (originalImage.ImageData is null)
            //{
            //    ValidationFailure validationFailure = new(nameof(ProductImage.ImageData), $"Image Data cannot be empty, Product Id: {originalImagesByProduct.Key}");

            //    return new ValidationResult([validationFailure]);
            //}

            if (originalImage.ProductId is null)
            {
                return new UnexpectedFailureResult();
            }

            OneOf<string, ValidationResult> getFileExtensionResult
                = _fileExtensionFromContentTypeService.GetFileExtensionFromImageContentType(originalImage.ImageContentType);

            if (!getFileExtensionResult.IsT0)
            {
                return getFileExtensionResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    fileExtension => new UnexpectedFailureResult(),
                    validationResult => validationResult);
            }

            ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest = new()
            {
                ExistingImageId = null,
                FileExtension = getFileExtensionResult.AsT0,
                ImageData = originalImage.ImageData,
                HtmlData = originalImage.HtmlData,
                FileUpsertRequest = new()
                {
                    Active = true,
                    CustomDisplayOrder = null,
                }
            };

            productImageWithFileUpsertRequests.Add(productImageWithFileUpsertRequest);
        }

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _productImageService.UpsertFirstAndAllImagesWithFilesForProductAsync(
                originalImagesByProduct.Key!.Value,
                productImageWithFileUpsertRequests,
                upsertUserName);

        if (productTestMetrics is not null)
        {
            productTestMetrics.AddProcessedProductId(originalImagesByProduct.Key.Value);

            ProductImage? productFirstImage = await _productImageService.GetByProductIdInFirstImagesAsync(originalImagesByProduct.Key.Value);
            List<ProductImage> newProductImages = await _productImageService.GetAllInProductAsync(originalImagesByProduct.Key.Value);

            if (productFirstImage is not null)
            {
                productTestMetrics.AddProcessedImagesId(productFirstImage.Id);
            }

            foreach (ProductImage newProductImage in newProductImages)
            {
                productTestMetrics.AddProcessedImagesAllId(newProductImage.Id);
            }
        }

        return result;
    }

    public async Task<IStatusCodeActionResult> OnGetSaveProductPropertiesFromOriginalImageHtmlAsync()
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        List<Product> allProducts = await _productService.GetAllAsync();

        ProductTestMetrics productTestMetrics = new();

        List<IGrouping<Product?, ProductImageData>> productsWithImages = await GetAllOrFirstOriginalImagesForProductsAsync(allProducts);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await SaveProductPropertiesFromOriginalImageHtmlAsync(productsWithImages, currentUserName, productTestMetrics);

        return result.Match<IStatusCodeActionResult>(
            success => new JsonResult(new { metrics = productTestMetrics }),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }


    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveProductPropertiesFromOriginalImageHtmlAsync(
        List<IGrouping<Product?, ProductImageData>> productsWithImages,
        string currentUserName,
        ProductTestMetrics? productTestMetrics = null)
    {
        TimeSpan timeout = TimeSpan.FromDays(2);

        TEMP__AllowTransactionScopesWithLongTimeouts(timeout);

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => SaveProductPropertiesFromOriginalImageHtmlInternalAsync(productsWithImages, currentUserName, productTestMetrics),
            result => result.IsT0,
            transactionOptions: new()
            {
                Timeout = timeout
            });
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveProductPropertiesFromOriginalImageHtmlInternalAsync(
        List<IGrouping<Product?, ProductImageData>> productsWithImages,
        string currentUserName,
        ProductTestMetrics? productTestMetrics = null)
    {
        foreach (IGrouping<Product?, ProductImageData> imagesByProduct in productsWithImages)
        {
            if (imagesByProduct.Key is null) continue;

            ProductImageData? imageDataWithHtml = imagesByProduct.FirstOrDefault(image => !string.IsNullOrWhiteSpace(image.HtmlData));

            if (imageDataWithHtml is null) continue;

            LegacyHtmlProduct htmlProduct = _legacyProductHtmlService.ParseProductHtml(imageDataWithHtml.HtmlData!);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertProductPropertiesFromHtmlResult
                = await UpsertPropertiesFromLegacyHtmlAsync(imagesByProduct.Key, htmlProduct, currentUserName, productTestMetrics);

            if (!upsertProductPropertiesFromHtmlResult.IsT0)
            {
                return upsertProductPropertiesFromHtmlResult;
            }


        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveUnsavedProductPropertiesFromOriginalImageHtmlInternalAsync(
        List<IGrouping<Product?, ProductImageData>> productsWithImages,
        List<IGrouping<int, ProductProperty>> propertiesOfProducts,
        string currentUserName,
        ProductTestMetrics? productTestMetrics = null)
    {
        foreach (IGrouping<Product?, ProductImageData> imagesByProduct in productsWithImages)
        {
            if (imagesByProduct.Key is null) continue;

            ProductImageData? imageDataWithHtml = imagesByProduct.FirstOrDefault(image => !string.IsNullOrWhiteSpace(image.HtmlData));

            if (imageDataWithHtml is null) continue;

            LegacyHtmlProduct htmlProduct = _legacyProductHtmlService.ParseProductHtml(imageDataWithHtml.HtmlData!);

            IGrouping<int, ProductProperty>? productProperties = propertiesOfProducts
                .FirstOrDefault(group => group.Key == imageDataWithHtml.ProductId);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertProductPropertiesFromHtmlResult
                = await UpsertUnsavedPropertiesFromLegacyHtmlAsync(
                imagesByProduct.Key, htmlProduct, productProperties, currentUserName, productTestMetrics);

            if (!upsertProductPropertiesFromHtmlResult.IsT0)
            {
                return upsertProductPropertiesFromHtmlResult;
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertPropertiesFromLegacyHtmlAsync(
        Product product, LegacyHtmlProduct htmlProduct, string currentUserName, ProductTestMetrics? productTestMetrics = null)
    {
        if (product.CategoryId is null)
        {
            ValidationFailure categoryIdError = new(nameof(product.CategoryId), "Cannot be null");

            return new ValidationResult([categoryIdError]);
        }

        if (htmlProduct.Properties is null
            || htmlProduct.Properties.Count <= 0)
        {
            ValidationFailure categoryIdError = new(nameof(htmlProduct.Properties), "Cannot be empty");

            return new ValidationResult([categoryIdError]);
        }

        List<ProductCharacteristicAndExternalXmlDataRelation> propertyRelationshipsFromProductCategory
            = await _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdAsync(product.CategoryId.Value);

        foreach (LegacyHtmlProductProperty htmlProperty in htmlProduct.Properties)
        {
            if (htmlProperty.Value is not null
                && string.IsNullOrWhiteSpace(htmlProperty.Value))
            {
                continue;
            }

            ProductCharacteristicAndExternalXmlDataRelation? propertyRelationship = propertyRelationshipsFromProductCategory
                .FirstOrDefault(x => x.XmlName == htmlProperty.Name);

            if (propertyRelationship is null
                || propertyRelationship.ProductCharacteristicId is null)
            {
                continue;
            }

            OneOf<Success, ValidationResult, UnexpectedFailureResult> insertPropertyActionResult = await UpsertXmlPropertyInPropertiesAsync(
                product.Id, htmlProperty, propertyRelationship, currentUserName);

            if (!insertPropertyActionResult.IsT0) return insertPropertyActionResult;

            if (productTestMetrics is not null)
            {
                productTestMetrics.AddProcessedProductId(product.Id);
                productTestMetrics.AddProcessedHtmlProperty(htmlProperty);
                productTestMetrics.AddProcessedProductPropertyForProduct(product.Id, propertyRelationship.ProductCharacteristicId.Value);
            }
        }

        if (htmlProduct.VendorUrl is not null)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> saveLinkResult
                = await SaveVendorUrlLinkFromXmlProductAsync(product.Id, htmlProduct.VendorUrl, currentUserName);

            if (productTestMetrics is not null)
            {
                productTestMetrics.AddProcessedProductId(product.Id);
                productTestMetrics.AddProcessedProductPropertyForProduct(product.Id, _defaultLinkCharacteristicId);
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertUnsavedPropertiesFromLegacyHtmlAsync(
        Product product,
        LegacyHtmlProduct htmlProduct,
        IEnumerable<ProductProperty>? productProperties,
        string currentUserName,
        ProductTestMetrics? productTestMetrics = null)
    {
        if (product.CategoryId is null)
        {
            ValidationFailure categoryIdError = new(nameof(product.CategoryId), "Cannot be null");

            return new ValidationResult([categoryIdError]);
        }

        if (htmlProduct.Properties is null
            || htmlProduct.Properties.Count <= 0)
        {
            ValidationFailure propertiesError = new(nameof(htmlProduct.Properties), "Cannot be empty");

            return new ValidationResult([propertiesError]);
        }

        List<ProductCharacteristicAndExternalXmlDataRelation> propertyRelationshipsFromProductCategory
            = await _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdAsync(product.CategoryId.Value);

        foreach (LegacyHtmlProductProperty htmlProperty in htmlProduct.Properties)
        {
            if (htmlProperty.Value is not null
                && string.IsNullOrWhiteSpace(htmlProperty.Value))
            {
                continue;
            }

            ProductCharacteristicAndExternalXmlDataRelation? propertyRelationship = propertyRelationshipsFromProductCategory
                .FirstOrDefault(x => x.XmlName == htmlProperty.Name);

            if (propertyRelationship is null
                || propertyRelationship.ProductCharacteristicId is null)
            {
                continue;
            }

            ProductProperty? existingProductProperty = productProperties?
                .FirstOrDefault(property => property.ProductCharacteristicId == propertyRelationship.ProductCharacteristicId);

            if (existingProductProperty is not null) continue;

            OneOf<Success, ValidationResult, UnexpectedFailureResult> insertPropertyActionResult = await UpsertXmlPropertyInPropertiesAsync(
                product.Id, htmlProperty, propertyRelationship, currentUserName);

            if (!insertPropertyActionResult.IsT0) return insertPropertyActionResult;

            if (productTestMetrics is not null)
            {
                productTestMetrics.AddProcessedProductId(product.Id);
                productTestMetrics.AddProcessedHtmlProperty(htmlProperty);
                productTestMetrics.AddProcessedProductPropertyForProduct(product.Id, propertyRelationship.ProductCharacteristicId.Value);
            }
        }

        if (htmlProduct.VendorUrl is not null)
        {
            ProductProperty? existingVendorUrlProperty = productProperties?
                .FirstOrDefault(property => property.ProductCharacteristicId == _defaultLinkCharacteristicId);

            if (existingVendorUrlProperty is null)
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult> saveLinkResult
                    = await SaveVendorUrlLinkFromXmlProductAsync(product.Id, htmlProduct.VendorUrl, currentUserName);

                if (productTestMetrics is not null)
                {
                    productTestMetrics.AddProcessedProductId(product.Id);
                    productTestMetrics.AddProcessedProductPropertyForProduct(product.Id, _defaultLinkCharacteristicId);
                }
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertXmlPropertyInPropertiesAsync(
        int productId,
        LegacyHtmlProductProperty htmlProperty,
        ProductCharacteristicAndExternalXmlDataRelation propertyRelationship,
        string upsertUserName)
    {
        if (propertyRelationship.ProductCharacteristicId is null)
        {
            ValidationFailure productCharacteristicIdError = new(
                nameof(ProductCharacteristicAndExternalXmlDataRelation.ProductCharacteristicId),
                "Cannot be null");

            return new ValidationResult([productCharacteristicIdError]);
        }

        ProductPropertyUpdateRequest productPropertyUpdateRequest = new()
        {
            ProductCharacteristicId = propertyRelationship.ProductCharacteristicId.Value,
            ProductId = productId,
            XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            Value = htmlProperty.Value,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertPropertyResult
            = await _productPropertyService.UpsertAsync(productPropertyUpdateRequest, upsertUserName);

        return upsertPropertyResult;
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveVendorUrlLinkFromXmlProductAsync(
        int productId, string? vendorUrl, string upsertUserName)
    {
        if (vendorUrl is null)
        {
            ValidationFailure emptyUrlError = new(nameof(vendorUrl), "Cannot be empty");

            return new ValidationResult([emptyUrlError]);
        }

        ProductCharacteristic? defaultLinkCharacteristic = await _productCharacteristicService.GetByIdAsync(_defaultLinkCharacteristicId);

        if (defaultLinkCharacteristic is null) return new UnexpectedFailureResult();

        ProductPropertyUpdateRequest updateRequest = new()
        {
            ProductId = productId,
            ProductCharacteristicId = defaultLinkCharacteristic.Id,
            Value = vendorUrl,
            XmlPlacement = null,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> createLinkResult
            = await _productPropertyService.UpsertAsync(updateRequest, upsertUserName);

        return createLinkResult;
    }

    private async Task<List<IGrouping<Product?, ProductImageData>>> GetAllOrFirstOriginalImagesForProductsAsync(List<Product> allProducts)
    {
        IEnumerable<int> productIds = allProducts.Select(product => product.Id);

        List<IGrouping<int, ProductImageData>> allOriginalProductImages = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);

        List<int> productsWithNoImagesInImagesAllIds = new();

        foreach (int productId in productIds)
        {
            IGrouping<int, ProductImageData>? productImages = allOriginalProductImages.FirstOrDefault(x => x.Key == productId);

            if (productImages is not null) continue;

            productsWithNoImagesInImagesAllIds.Add(productId);
        }

        List<ProductImageData> allImagesList = allOriginalProductImages
            .SelectMany(x => x)
            .ToList();

        List<ProductImage> productsWithNoImagesInImagesAllFirstImages
            = await _productImageService.GetFirstImagesForSelectionOfProductsAsync(productsWithNoImagesInImagesAllIds);

        foreach (ProductImage firstImage in productsWithNoImagesInImagesAllFirstImages)
        {
            ProductImageData firstImageData = new()
            {
                Id = firstImage.Id,
                ProductId = firstImage.ProductId,
                ImageContentType = firstImage.ImageContentType,
                HtmlData = firstImage.HtmlData,
                DateModified = firstImage.DateModified,
            };

            allImagesList.Add(firstImageData);
        }

        List<IGrouping<Product?, ProductImageData>> productsWithImages = allImagesList
            .GroupBy(image => allProducts.FirstOrDefault(product => product.Id == image.ProductId))
            .ToList();

        return productsWithImages;
    }

    private async Task<List<IGrouping<Product?, ProductImage>>> GetAllOrFirstOriginalImagesForProductsWithDataAsync(List<Product> requiredProducts)
    {
        IEnumerable<int> productIds = requiredProducts.Select(product => product.Id);

        List<IGrouping<int, ProductImage>> allOriginalProductImages = await _productImageService.GetAllInProductsAsync(productIds);

        List<int> productsWithNoImagesInImagesAllIds = new();

        foreach (int productId in productIds)
        {
            IGrouping<int, ProductImage>? productImages = allOriginalProductImages.FirstOrDefault(x => x.Key == productId);

            if (productImages is not null) continue;

            productsWithNoImagesInImagesAllIds.Add(productId);
        }

        List<ProductImage> allImagesList = allOriginalProductImages
            .SelectMany(x => x)
            .ToList();

        List<ProductImage> productsWithNoImagesInImagesAllFirstImages
            = await _productImageService.GetFirstImagesForSelectionOfProductsAsync(productsWithNoImagesInImagesAllIds);

        foreach (ProductImage firstImage in productsWithNoImagesInImagesAllFirstImages)
        {
            allImagesList.Add(firstImage);
        }

        List<IGrouping<Product?, ProductImage>> productsWithImages = allImagesList
            .GroupBy(image => requiredProducts.FirstOrDefault(product => product.Id == image.ProductId))
            .ToList();

        return productsWithImages;
    }

    private static bool IsImageDataValid(Stream imageDataStream)
    {
        try
        {
            Image.Load(imageDataStream);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public sealed class ProductTestMetrics
{
    private readonly List<int> _processedProductIds = new();
    private readonly List<int> _processedImagesIds = new();
    private readonly List<int> _processedImagesAllIds = new();
    private readonly List<Tuple<int, List<int>>> _processedProductPropertyData = new();
    private readonly List<LegacyHtmlProductProperty> _processedHtmlProperties = new();

    public IReadOnlyList<int> ProcessedProductIds => _processedProductIds;
    public IReadOnlyList<int> ProcessedImagesIds => _processedImagesIds;
    public IReadOnlyList<int> ProcessedImagesAllIds => _processedImagesAllIds;
    public IReadOnlyList<Tuple<int, List<int>>> ProcessedProductPropertyData => _processedProductPropertyData;
    public IReadOnlyList<LegacyHtmlProductProperty> ProcessedHtmlProperties => _processedHtmlProperties;

    public void AddProcessedProductId(int id)
    {
        if (!_processedProductIds.Contains(id))
        {
            _processedProductIds.Add(id);
        }
    }

    public void RemoveProcessedProductId(int id)
    {
        _processedProductIds.Remove(id);
    }

    public void AddProcessedImagesId(int id)
    {
        if (!_processedImagesIds.Contains(id))
        {
            _processedImagesIds.Add(id);
        }
    }

    public void RemoveProcessedImagesId(int id)
    {
        _processedImagesIds.Remove(id);
    }

    public void AddProcessedImagesAllId(int id)
    {
        if (!_processedImagesAllIds.Contains(id))
        {
            _processedImagesAllIds.Add(id);
        }
    }

    public void RemoveProcessedImagesAllId(int id)
    {
        _processedImagesAllIds.Remove(id);
    }

    public void AddProcessedProductPropertyForProduct(int productId, int characteristicId)
    {
        Tuple<int, List<int>>? existingEntry = _processedProductPropertyData
            .FirstOrDefault(x => x.Item1 == productId);

        if (existingEntry is not null
            && !existingEntry.Item2.Contains(characteristicId))
        {
            existingEntry.Item2.Add(characteristicId);

            return;
        }

        _processedProductPropertyData.Add(new (productId, new List<int> { characteristicId }));
    }

    public void RemoveProcessedProductPropertyForProduct(int productId, int characteristicId)
    {
        Tuple<int, List<int>>? existingEntry = _processedProductPropertyData
            .FirstOrDefault(x => x.Item1 == productId);

        if (existingEntry is null) return;

        existingEntry.Item2.Remove(characteristicId);
    }

    public void RemoveAllProcessedProductPropertiesForProduct(int productId)
    {
        int existingEntryIndex = _processedProductPropertyData
            .FindIndex(x => x.Item1 == productId);

        if (existingEntryIndex < 0) return;

        _processedProductPropertyData.RemoveAt(existingEntryIndex);
    }

    public void AddProcessedHtmlProperty(LegacyHtmlProductProperty property)
    {
        if (!_processedHtmlProperties.Contains(property))
        {
            _processedHtmlProperties.Add(property);
        }
    }

    public void RemoveProcessedHtmlProperty(LegacyHtmlProductProperty property)
    {
        _processedHtmlProperties.Remove(property);
    }
}