using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductCharacteristicsComparer;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;

using static MOSTComputers.UI.Web.RealWorkTesting.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductCharacteristicComparerMappingUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages;

[Authorize]
public class ProductCharacteristicsComparerModel : PageModel
{
    public ProductCharacteristicsComparerModel(
        IHttpClientFactory httpClientFactory,
        IProductXmlProvidingService productXmlProvidingService,
        IWebHostEnvironment webHostEnvironment,
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        ICategoryService categoryService,
        IProductService productService,
        IProductImageService productImageService,
        IProductCharacteristicAndExternalXmlDataRelationService productCharacteristicAndExternalXmlDataRelationService,
        IXmlImportProductPropertyService xmlImportProductPropertyService,
        IXmlImportProductImageFileNameInfoService xmlImportProductImageFileNameInfoService,
        IXmlImportProductImageService xmlImportProductImageService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageFileManagementService productImageFileManagementService,
        IProductDeserializeService productDeserializeService,
        IProductHtmlService productHtmlService,
        IProductToXmlProductMappingService productToXmlProductMappingService,
        IXmlProductToProductMappingService xmlProductToProductMappingService,
        IProductXmlDataSaveService productRelationDataSaveService,
        ITransactionExecuteService transactionExecuteService)
    {
        _httpClientFactory = httpClientFactory;
        _productXmlProvidingService = productXmlProvidingService;
        _webHostEnvironment = webHostEnvironment;
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _categoryService = categoryService;
        _productService = productService;
        _productImageService = productImageService;
        _productCharacteristicAndExternalXmlDataRelationService = productCharacteristicAndExternalXmlDataRelationService;
        _xmlImportProductPropertyService = xmlImportProductPropertyService;
        _xmlImportProductImageFileNameInfoService = xmlImportProductImageFileNameInfoService;
        _xmlImportProductImageService = xmlImportProductImageService;
        _productCharacteristicService = productCharacteristicService;
        _productImageFileManagementService = productImageFileManagementService;
        _productDeserializeService = productDeserializeService;
        _productHtmlService = productHtmlService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
        _xmlProductToProductMappingService = xmlProductToProductMappingService;
        _productRelationDataSaveService = productRelationDataSaveService;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IProductXmlProvidingService _productXmlProvidingService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductCharacteristicAndExternalXmlDataRelationService _productCharacteristicAndExternalXmlDataRelationService;

    private readonly IXmlImportProductPropertyService _xmlImportProductPropertyService;
    private readonly IXmlImportProductImageFileNameInfoService _xmlImportProductImageFileNameInfoService;
    private readonly IXmlImportProductImageService _xmlImportProductImageService;

    private readonly IProductCharacteristicService _productCharacteristicService;

    private readonly IProductImageFileManagementService _productImageFileManagementService;

    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductHtmlService _productHtmlService;

    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;
    private readonly IXmlProductToProductMappingService _xmlProductToProductMappingService;
    private readonly IProductXmlDataSaveService _productRelationDataSaveService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public List<Product> LocalProducts { get; } = new();
    public List<XmlProduct> ExternalProducts { get; } = new();
    
    public static List<LocalCharacteristicDisplayData> LocalDistinctCharacteristics { get; private set; } = new();
    public static List<ExternalXmlPropertyDisplayData> ExternalDistinctProperties { get; private set; } = new();

    public static List<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; private set; } = new();

    public async Task<IActionResult> OnPostGetLocalAndExternalXmlDataForCategoryAsync(int categoryId)
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        return getProductXmlResult.Match(
            productXml =>
            {
                XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

                if (productDeserializeResult?.Products is not null
                    && productDeserializeResult.Products.Count > 0)
                {
                    productDeserializeResult.Products = productDeserializeResult.Products.FindAll(x => x.Category?.Id == categoryId);

                    return GetLocalAndExternalXmlDataForCategory(categoryId, productDeserializeResult.Products);
                }

                return new OkResult();
            },
            notFound => StatusCode(500));
    }

    private IActionResult GetLocalAndExternalXmlDataForCategory(int categoryId, List<XmlProduct> xmlProducts)
    {
        IEnumerable<Product> productsFromSameCategory = _productService.GetAllWithoutImagesAndProps()
            .Where(product => product.CategoryId == categoryId)
            .OrderByDescending(product => xmlProducts.FirstOrDefault(
                xmlProduct => xmlProduct.Id == product.Id) is not null)
            .ThenBy(product => product.Id);

        xmlProducts = xmlProducts.OrderByDescending(
            xmlProduct => productsFromSameCategory.FirstOrDefault(
                product => product.Id == xmlProduct.Id) is not null)
            .ThenBy(xmlProduct => xmlProduct.Id)
            .ToList();

        foreach (Product product in productsFromSameCategory)
        {
            IEnumerable<XmlImportProductImage> productTestImages = _xmlImportProductImageService.GetAllInProduct(product.Id);
            IEnumerable<XmlImportProductImageFileNameInfo> productTestImageFileNames = _xmlImportProductImageFileNameInfoService.GetAllInProduct(product.Id);
            IEnumerable<XmlImportProductProperty> productTestProperties = _xmlImportProductPropertyService.GetAllInProduct(product.Id);

            product.Images = productTestImages.Select(x => Map(x))
                .ToList();

            product.ImageFileNames = productTestImageFileNames.Select(x => Map(x))
                .ToList();

            product.Properties = productTestProperties.Select(x => Map(x))
                .ToList();
        }

        List<XmlProduct> localProductsToXml = new();

        foreach (Product product in productsFromSameCategory)
        {
            XmlProduct localProductToXml = _productToXmlProductMappingService.MapToXmlProduct(product);

            localProductsToXml.Add(localProductToXml);
        }

        XmlObjectData productsToXmlData = new()
        {
            Products = localProductsToXml,
        };

        OneOf<string, InvalidXmlResult> localXmlProductsSerializeResult = _productDeserializeService.TrySerializeProductsXml(productsToXmlData, true);

        return localXmlProductsSerializeResult.Match(
            localXml =>
            {
                XmlObjectData xmlObjectData = new()
                {
                    Products = xmlProducts
                };

                OneOf<string, InvalidXmlResult> externalXmlProductsSerializeResult
                    = _productDeserializeService.TrySerializeProductsXml(xmlObjectData, true);

                return externalXmlProductsSerializeResult.Match<IStatusCodeActionResult>(
                    externalXml => GetLocalAndExternalXmlTablePartialView(localXml, externalXml),
                    invalidXmlResult => BadRequest(invalidXmlResult));
            },
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    public async Task<IActionResult> OnPostCompareExternalAndLocalDataAsync([FromBody] string externalXml)
    {
        if (string.IsNullOrEmpty(externalXml)) return BadRequest();

        XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(externalXml);

        return await CompareExternalAndLocalDataAsync(
            productDeserializeResult, excludeLocalCharacteristicsWithNoMatches: true, addBaseCategoryCharacteristics: true);
    }

    public async Task<IStatusCodeActionResult> OnPostCompareExternalAndLocalDataFromFileAsync()
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        return await getProductXmlResult.Match(
            productXml =>
            {
                XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

                return CompareExternalAndLocalDataAsync(
                    productDeserializeResult, excludeLocalCharacteristicsWithNoMatches: true, addBaseCategoryCharacteristics: true);
            },
            notFound => Task.FromResult<IStatusCodeActionResult>(StatusCode(500)));
    }

    public async Task<IActionResult> OnPostCompareExternalAndLocalDataForCategoryFromFileAsync(int categoryId)
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        return await getProductXmlResult.Match(
            productXml =>
            {
                XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

                if (productDeserializeResult?.Products is not null
                    && productDeserializeResult.Products.Count > 0)
                {
                    productDeserializeResult.Products = productDeserializeResult.Products.FindAll(x => x.Category?.Id == categoryId);
                }

                List<ProductCharacteristicAndExternalXmlDataRelation> alreadyAddedRelations
                = _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryId(categoryId);

                return CompareExternalAndLocalDataAsync(
                    productDeserializeResult,
                    excludeLocalCharacteristicsWithNoMatches: false,
                    addBaseCategoryCharacteristics: false,
                    alreadyAddedRelations: alreadyAddedRelations);
            },
            notFound => Task.FromResult<IStatusCodeActionResult>(StatusCode(500)));
    }

    public IActionResult OnPostAddNewEmptyRelationship()
    {
        ExternalAndLocalCharacteristicRelations ??= new();

        ExternalAndLocalCharacteristicRelationDisplayData newRelationship = new();

        ExternalAndLocalCharacteristicRelations.Add(newRelationship);

        return GetCharacteristicsRelationshipTablePartialView(ExternalAndLocalCharacteristicRelations);
    }

    public IActionResult OnPostSaveAllRelationships(bool createNewCharacteristicsForMissingRelationships = false)
    {
        return _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
            SaveAllRelationshipsInternal,
            result => result.StatusCode == 200,
            createNewCharacteristicsForMissingRelationships);
    }

    private IStatusCodeActionResult SaveAllRelationshipsInternal(bool createNewCharacteristicsForMissingRelationships)
    {
        List<Category> allCategories = GetAllCategories();

        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in ExternalAndLocalCharacteristicRelations)
        {
            if (relationDisplayData.MatchingExternalProperties is null
                || relationDisplayData.MatchingExternalProperties.Count <= 0)
            {
                continue;
            }

            bool doesRelationContainIndexing = (relationDisplayData.CorrespondingCharacteristicAndXmlPropertyIndexes?.Count > 0);

            if (relationDisplayData.MatchingLocalCharacteristics?.Count == 1
                && relationDisplayData.MatchingExternalProperties.Count > 0
                && !doesRelationContainIndexing)
            {
                LocalCharacteristicDisplayData characteristicDataInRelation = relationDisplayData.MatchingLocalCharacteristics[0];

                Category? matchingCategory = allCategories.FirstOrDefault(x => x.Id == characteristicDataInRelation.CategoryId);

                foreach (ExternalXmlPropertyDisplayData externalXmlProperty in relationDisplayData.MatchingExternalProperties)
                {
                    if (matchingCategory is null) return BadRequest();

                    IStatusCodeActionResult upsertRelationResult = UpsertNewRelationFromData(
                        matchingCategory.Description,
                        characteristicDataInRelation.Id,
                        characteristicDataInRelation.Name,
                        characteristicDataInRelation.Meaning,
                        externalXmlProperty);

                    if (upsertRelationResult.StatusCode != 200)
                    {
                        return BadRequest();
                    }
                }

                continue;
            }

            for (int i = 0; i < relationDisplayData.MatchingExternalProperties.Count; i++)
            {
                ExternalXmlPropertyDisplayData externalPropertyInRelation = relationDisplayData.MatchingExternalProperties[i];

                KeyValuePair<int, List<int>> defaultValue = new(-1, null!);

                int? matchingLocalCharacteristicIndex = relationDisplayData.CorrespondingCharacteristicAndXmlPropertyIndexes?
                    .FirstOrDefault(kvp => kvp.Value.Contains(i), defaultValue)
                    .Key;

                LocalCharacteristicDisplayData? matchingCharacteristic = null;

                if (matchingLocalCharacteristicIndex is not null
                    && matchingLocalCharacteristicIndex != -1)
                {
                    matchingCharacteristic = relationDisplayData.MatchingLocalCharacteristics?[matchingLocalCharacteristicIndex.Value];
                }

                int? characteristicId = matchingCharacteristic?.Id;
                string? characteristicName = matchingCharacteristic?.Name;
                string? characteristicMeaning = matchingCharacteristic?.Meaning;

                if (matchingCharacteristic is null
                    && createNewCharacteristicsForMissingRelationships)
                {
                    ProductCharacteristicCreateRequest productCharacteristicCreateRequest = new()
                    {
                        CategoryId = externalPropertyInRelation.CategoryId,
                        Name = externalPropertyInRelation.Name,
                        DisplayOrder = externalPropertyInRelation.Order,
                        Active = false,
                        KWPrCh = ProductCharacteristicTypeEnum.ProductCharacteristic,
                    };

                    OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(productCharacteristicCreateRequest);

                    bool isCharacteristicInsertSuccessful = characteristicInsertResult.Match(
                        id => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    characteristicId = characteristicInsertResult.AsT0;
                    characteristicName = productCharacteristicCreateRequest.Name;
                    characteristicMeaning = productCharacteristicCreateRequest.Meaning;

                    if (!isCharacteristicInsertSuccessful)
                    {
                        return BadRequest();
                    }
                }

                Category? matchingCategory = allCategories.FirstOrDefault(x => x.Id == externalPropertyInRelation.CategoryId);

                if (matchingCategory is null) return BadRequest();

                IStatusCodeActionResult upsertRelationResult
                    = UpsertNewRelationFromData(
                        matchingCategory.Description,
                        characteristicId,
                        characteristicName,
                        characteristicMeaning,
                        externalPropertyInRelation);

                if (upsertRelationResult.StatusCode != 200)
                {
                    return BadRequest();
                }
            }
        }

        return new OkResult();
    }

    public async Task<IActionResult> OnPostSaveAllPropertiesForCategoryAsync(int categoryId)
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        return getProductXmlResult.Match(
            productXml =>
            {
                XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

                if (productDeserializeResult?.Products is not null
                    && productDeserializeResult.Products.Count > 0)
                {
                    productDeserializeResult.Products = productDeserializeResult.Products.FindAll(x => x.Category?.Id == categoryId);

                    List<ProductCharacteristicAndExternalXmlDataRelation> alreadyAddedRelations
                        = _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryId(categoryId);

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertPropertiesResult
                        = _productRelationDataSaveService.UpsertProductPropertiesFromXmlPropertiesBasedOnRelationData(
                            productDeserializeResult.Products, alreadyAddedRelations);

                    return upsertPropertiesResult.Match(
                        success => new OkResult(),
                        validationResult => GetBadRequestResultFromValidationResult(validationResult),
                        unexpectedFailureResult => StatusCode(500));
                }

                return new OkResult();
            },
            notFound => StatusCode(500));
    }

    public async Task<IActionResult> OnPostSaveAllImageFileNamesForCategoryAsync(int categoryId)
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        if (getProductXmlResult.IsT1)
        {
            return NotFound(getProductXmlResult.AsT1); 
        }

        string productXml = getProductXmlResult.AsT0;

        XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

        if (productDeserializeResult?.Products is not null
            && productDeserializeResult.Products.Count > 0)
        {
            productDeserializeResult.Products = productDeserializeResult.Products.FindAll(x => x.Category?.Id == categoryId);

            OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult> upsertImageFileNamesAndFilesResult
                = await _productRelationDataSaveService.UpsertImageFileNamesAndFilesFromXmlDataAsync(
                    productDeserializeResult.Products, insertFiles: true);

            return upsertImageFileNamesAndFilesResult.Match(
                success => new OkResult(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(500),
                notSupportedFileTypeResult => BadRequest(notSupportedFileTypeResult),
                directoryNotFoundResult => StatusCode(500));
        }

        return new OkResult();
    }

    public async Task<IActionResult> OnPostSaveAllImagesForTestingForCategoryAsync(int categoryId)
    {
        OneOf<string, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlAsync();

        if (getProductXmlResult.IsT1)
        {
            return NotFound(getProductXmlResult.AsT1);
        }

        string productXml = getProductXmlResult.AsT0;

        XmlObjectData? productDeserializeResult = _productDeserializeService.DeserializeProductsXml(productXml);

        if (productDeserializeResult?.Products is not null
            && productDeserializeResult.Products.Count > 0)
        {
            productDeserializeResult.Products = productDeserializeResult.Products.FindAll(x => x.Category?.Id == categoryId);

            OneOf<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult> result
                = await _productRelationDataSaveService.UpsertTestingImagesFromXmlDataAsync(productDeserializeResult.Products);

            return result.Match(
                success => new OkResult(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                invalidXmlResult => BadRequest(invalidXmlResult),
                unexpectedFailureResult => StatusCode(500));
        }

        return new OkResult();
    }

    public IStatusCodeActionResult UpsertNewRelationFromData(
        string? categoryName,
        int? characteristicId,
        string? characteristicName,
        string? characteristicMeaning,
        ExternalXmlPropertyDisplayData externalXmlProperty)
    {
        ProductCharacteristicAndExternalXmlDataRelationUpsertRequest productCharacteristicAndExternalXmlDataRelationUpsertRequest = new()
        {
            CategoryId = externalXmlProperty.CategoryId,
            CategoryName = categoryName,
            ProductCharacteristicId = characteristicId,
            ProductCharacteristicName = characteristicName,
            ProductCharacteristicMeaning = characteristicMeaning,
            XmlName = externalXmlProperty.Name,
            XmlDisplayOrder = externalXmlProperty.Order,
        };

        OneOf<Success, UnexpectedFailureResult> upsertRelationResult
            = _productCharacteristicAndExternalXmlDataRelationService.UpsertByCharacteristicId(productCharacteristicAndExternalXmlDataRelationUpsertRequest);

        return upsertRelationResult.Match(
            id => new OkResult(),
            unexpectedFailureResult => StatusCode(500));
    }

    public IActionResult OnPutMatchCorrespondingPropertyAndCharacteristic(
        int relationshipIndex,
        int characteristicIndex,
        int externalXmlPropertyIndex,
        [FromBody] BackgroundColorDTO? newBackgroundColorOfElements = null)
    {
        if (relationshipIndex < 0
           || relationshipIndex >= ExternalAndLocalCharacteristicRelations.Count)
        {
            return BadRequest();
        }

        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMatchIn = ExternalAndLocalCharacteristicRelations[relationshipIndex];

        Color? backgroundColor = null;

        if (newBackgroundColorOfElements is not null)
        {
            backgroundColor = Color.FromArgb(
                255, newBackgroundColorOfElements.Red, newBackgroundColorOfElements.Green, newBackgroundColorOfElements.Blue);
        }

        bool matchingSuccess = relationshipToMatchIn.MatchCharacteristicAndPropertyAt(
            characteristicIndex, externalXmlPropertyIndex, backgroundColor);

        return matchingSuccess ? new OkResult() : BadRequest();
    }

    public IActionResult OnPutMoveItemsFromOneRelationshipToAnother(
        int indexOfRelationshipToMoveFrom,
        int indexOfRelationshipToMoveTo,
        LocalCharacteristicOrExternalPropertyEnum moveCharacteristicsOrProperties,
        [FromBody] int[] indexesOfItemsToMove)
    {
        if (indexesOfItemsToMove.Length == 0) return BadRequest();

        if (indexOfRelationshipToMoveFrom < 0
            || indexOfRelationshipToMoveFrom >= ExternalAndLocalCharacteristicRelations.Count
            || indexOfRelationshipToMoveTo < 0
            || indexOfRelationshipToMoveTo >= ExternalAndLocalCharacteristicRelations.Count)
        {
            return BadRequest();
        }

        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveFrom = ExternalAndLocalCharacteristicRelations[indexOfRelationshipToMoveFrom];
        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveTo = ExternalAndLocalCharacteristicRelations[indexOfRelationshipToMoveTo];

        return MoveFromOneRelatioshipToAnother(moveCharacteristicsOrProperties, relationshipToMoveFrom, relationshipToMoveTo, indexesOfItemsToMove);
    }

    private IActionResult MoveFromOneRelatioshipToAnother(
        LocalCharacteristicOrExternalPropertyEnum moveCharacteristicsOrProperties,
        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveFrom,
        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveTo,
        int[] indexesOfItemsToMove)
    {
        if (moveCharacteristicsOrProperties == LocalCharacteristicOrExternalPropertyEnum.Characteristic)
        {
            if (relationshipToMoveFrom.MatchingLocalCharacteristics is null
               || relationshipToMoveFrom.MatchingLocalCharacteristics.Count <= 0)
            {
                return (indexesOfItemsToMove.Length == 0) ? new OkResult() : BadRequest();
            }

            List<LocalCharacteristicDisplayData> characteristicsToMove = new();

            for (int i = 0; i < indexesOfItemsToMove.Length; i++)
            {
                int characteristicToMoveIndex = indexesOfItemsToMove[i];

                if (characteristicToMoveIndex < 0
                    || characteristicToMoveIndex >= relationshipToMoveFrom.MatchingLocalCharacteristics.Count)
                {
                    return BadRequest();
                }

                LocalCharacteristicDisplayData characteristicToMove = relationshipToMoveFrom.MatchingLocalCharacteristics[characteristicToMoveIndex];

                characteristicsToMove.Add(characteristicToMove);

                relationshipToMoveFrom.RemoveCharacteristicAt(characteristicToMoveIndex);

                for (int j = 0; j < indexesOfItemsToMove.Length; j++)
                {
                    if (indexesOfItemsToMove[j] > characteristicToMoveIndex)
                    {
                        indexesOfItemsToMove[j]--;
                    }
                }
            }

            relationshipToMoveTo.AddCharacteristics(characteristicsToMove);

            return GetCharacteristicsRelationshipTablePartialView(ExternalAndLocalCharacteristicRelations);
        }

        if (relationshipToMoveFrom.MatchingExternalProperties is null
           || relationshipToMoveFrom.MatchingExternalProperties.Count <= 0)
        {
            return (indexesOfItemsToMove.Length == 0) ? new OkResult() : BadRequest();
        }

        List<ExternalXmlPropertyDisplayData> externalPropertiesToMove = new();

        for (int i = 0; i < indexesOfItemsToMove.Length; i++)
        {
            int externalPropertyToMoveIndex = indexesOfItemsToMove[i];

            if (externalPropertyToMoveIndex < 0
                || externalPropertyToMoveIndex >= relationshipToMoveFrom.MatchingExternalProperties.Count)
            {
                return BadRequest();
            }

            ExternalXmlPropertyDisplayData externalPropertyToMove = relationshipToMoveFrom.MatchingExternalProperties[externalPropertyToMoveIndex];

            externalPropertiesToMove.Add(externalPropertyToMove);

            relationshipToMoveFrom.RemovePropertyAt(externalPropertyToMoveIndex);

            for (int j = 0; j < indexesOfItemsToMove.Length; j++)
            {
                if (indexesOfItemsToMove[j] > externalPropertyToMoveIndex)
                {
                    indexesOfItemsToMove[j]--;
                }
            }
        }

        relationshipToMoveTo.AddProperties(externalPropertiesToMove);

        return GetCharacteristicsRelationshipTablePartialView(ExternalAndLocalCharacteristicRelations);
    }

    public IActionResult OnDeleteDeleteSavedRelationsForCategory(int categoryId)
    {
        if (categoryId < -1) return BadRequest();

        bool isDeleteSuccessful = _productCharacteristicAndExternalXmlDataRelationService.DeleteAllWithSameCategoryId(categoryId);

        return isDeleteSuccessful ? new OkResult() : NotFound();
    }

    public List<Category> GetAllCategories()
    {
        return _categoryService.GetAll()
            .ToList();
    }

    private async Task<IStatusCodeActionResult> CompareExternalAndLocalDataAsync(
        XmlObjectData? productDeserializeResult,
        bool excludeLocalCharacteristicsWithNoMatches,
        bool addBaseCategoryCharacteristics,
        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation>? alreadyAddedRelations = null)
    {
        if (productDeserializeResult is null
            || productDeserializeResult.Products.Count <= 0)
        {
            return BadRequest();
        }

        ValidationResult xmlPropertiesValidationResult = ValidateAllXmlProperties(productDeserializeResult.Products);

        if (!xmlPropertiesValidationResult.IsValid)
        {
            return GetBadRequestResultFromValidationResult(xmlPropertiesValidationResult);
        }

        ExternalDistinctProperties = GetExternalDistinctProperties(productDeserializeResult);

        List<int> productIds = new();

        List<int> categoryIds = new() { };

        if (addBaseCategoryCharacteristics)
        {
            categoryIds.Add(-1);
        }

        foreach (XmlProduct xmlProduct in productDeserializeResult.Products)
        {
            if (productIds.Contains(xmlProduct.Id)) return BadRequest();

            productIds.Add(xmlProduct.Id);

            ExternalProducts.Add(xmlProduct);

            int categoryId = xmlProduct.Category.Id;

            if (!categoryIds.Contains(categoryId))
            {
                categoryIds.Add(categoryId);
            }
        }

        LocalDistinctCharacteristics = GetLocalCharacteristicDisplayData(categoryIds, ExternalDistinctProperties, excludeLocalCharacteristicsWithNoMatches);

        ExternalAndLocalCharacteristicRelations = GetCharacteristicRelations(
            LocalDistinctCharacteristics,
            ExternalDistinctProperties,
            excludeLocalCharacteristicsWithNoMatches,
            alreadyAddedRelations);

        List<Product>? localProducts = GetLocalProductData(productIds);

        if (localProducts is not null)
        {
            LocalProducts.AddRange(localProducts);
        }

        Task<string> localDataPartialViewTask = GetLocalCharacteristicsPartialViewAsString(LocalDistinctCharacteristics);
        Task<string> externalDataPartialViewTask = GetExternalPropertiesPartialViewAsString(ExternalDistinctProperties);
        Task<string> characteristicsRelationshipTablePartialViewTask = GetCharacteristicsRelationshipTablePartialViewAsString(ExternalAndLocalCharacteristicRelations);

        return new JsonResult(new
        {
            localCharacteristicsPartialViewText = await localDataPartialViewTask,
            externalPropertiesPartialViewText = await externalDataPartialViewTask,
            characteristicsRelationshipTablePartialViewText = await characteristicsRelationshipTablePartialViewTask
        });
    }

    private List<Product>? GetLocalProductData(List<int> productIds)
    {
        if (productIds.Count <= 0) return null;

        return _productService.GetSelectionWithProps(productIds)
            .ToList();
    }

    private static ValidationResult ValidateAllXmlProperties(List<XmlProduct> xmlProducts)
    {
        ValidationResult output = new();

        for (int i = 0; i < xmlProducts.Count; i++)
        {
            XmlProduct xmlProduct = xmlProducts[i];

            for (int j = 0; j < xmlProduct.XmlProductProperties.Count; j++)
            {
                XmlProductProperty xmlProperty = xmlProduct.XmlProductProperties[j];

                if (string.IsNullOrEmpty(xmlProperty.Order)) continue;

                bool isOrderAnInteger = int.TryParse(xmlProperty.Order, out int displayOrder);

                if (!isOrderAnInteger)
                {
                    output.Errors.Add(new(
                        $"{nameof(XmlProduct)}[{i}].{nameof(XmlProduct.XmlProductProperties)}[{j}].{nameof(XmlProductProperty.Order)}",
                        "Order must be an integer"));
                }
            }
        }

        return output;
    }

    private List<LocalCharacteristicDisplayData> GetLocalCharacteristicDisplayData(
        List<int> categoryIds,
        IEnumerable<ExternalXmlPropertyDisplayData> externalProperties,
        bool excludeLocalCharacteristicsWithNoMatches)
    {
        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsForAllRelevantCategories
            = _productCharacteristicService.GetCharacteristicsOnlyForSelectionOfCategoryIds(categoryIds);

        if (!characteristicsForAllRelevantCategories.Any()) return new();

        IEnumerable<ProductCharacteristic> matchingCharacteristics;

        if (!excludeLocalCharacteristicsWithNoMatches)
        {
            matchingCharacteristics = characteristicsForAllRelevantCategories
                .OrderBy(group => group.Key)
                .SelectMany(group => group
                    .Where(characteristic => characteristic.Active == true)
                    .OrderBy(characteristic => characteristic.Name)
                    .ThenBy(characteristic => characteristic.DisplayOrder));
        }
        else
        {
            matchingCharacteristics = characteristicsForAllRelevantCategories
                .OrderBy(group => group.Key)
                .SelectMany(group => group
                    .Where(characteristic =>
                    {
                        if (characteristic.Active != true) return false;

                        ExternalXmlPropertyDisplayData? matchingPropertyData = externalProperties.FirstOrDefault(externalProperty =>
                            externalProperty.CategoryId == characteristic.CategoryId
                                && externalProperty.Name == characteristic.Name);

                        return matchingPropertyData is not null;
                    })
                    .OrderBy(characteristic => characteristic.Name)
                    .ThenBy(characteristic => characteristic.DisplayOrder))
                .ToList();
        }

        return MapManyToLocalCharacteristicDisplayData(matchingCharacteristics);
    }

    private static List<ExternalXmlPropertyDisplayData> GetExternalDistinctProperties(XmlObjectData xmlObjectData)
    {
        List<ExternalXmlPropertyDisplayData> output = new();

        foreach (XmlProduct xmlProduct in xmlObjectData.Products)
        {
            IEnumerable<ExternalXmlPropertyDisplayData> externalXmlProperties = xmlProduct.XmlProductProperties.Select(
                xmlProperty => MapToExternalXmlPropertyDisplayData(xmlProduct.Category.Id, xmlProperty));

            output.AddRange(externalXmlProperties);
        }

        output = output.Distinct(new ExternalXmlPropertyDisplayDataEqualityComparerWithoutValueComparison())
            .OrderBy(x => x.CategoryId)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Order)
            .ToList();

        return output;
    }

    private List<ExternalAndLocalCharacteristicRelationDisplayData> GetCharacteristicRelations(
        List<LocalCharacteristicDisplayData> localDistinctCharacteristics,
        List<ExternalXmlPropertyDisplayData> externalDistinctProperties,
        bool excludeLocalCharacteristicsWithNoMatches,
        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation>? productCharacteristicAndExternalXmlPropertyRelations = null)
    {
        List<ExternalAndLocalCharacteristicRelationDisplayData> output = new();

        if (productCharacteristicAndExternalXmlPropertyRelations is not null)
        {
            List<ExternalAndLocalCharacteristicRelationDisplayData> relationDisplayDataFromRelationData
                = GetRelationshipDisplayDataFromRelationData(productCharacteristicAndExternalXmlPropertyRelations);

            output.AddRange(relationDisplayDataFromRelationData);
        }

        IEnumerable<IGrouping<Tuple<int, string?>, LocalCharacteristicDisplayData>> localCharacteristicsGrouped = localDistinctCharacteristics
            .Where(characteristic => !IsCharacteristicDataAlreadyAdded(characteristic, output))
            .GroupBy(characteristic => new Tuple<int, string?>(characteristic.CategoryId!.Value, characteristic.Name));

        List<IGrouping<Tuple<int, string?>, ExternalXmlPropertyDisplayData>> externalPropertiesGrouped = externalDistinctProperties
            .Where(externalXmlProperty => !IsExternalXmlPropertyDataAlreadyAdded(externalXmlProperty, output))
            .GroupBy(externalXmlProperty => new Tuple<int, string?>(externalXmlProperty.CategoryId, externalXmlProperty.Name))
            .ToList();

        foreach (IGrouping<Tuple<int, string?>, LocalCharacteristicDisplayData> localGrouping in localCharacteristicsGrouped)
        {
            ExternalAndLocalCharacteristicRelationDisplayData? relationToAddDataTo
                 = output.FirstOrDefault(x => localGrouping.Key.Item2 == IsCharacteristicMatchedWithRelationshipByName(x));

            IGrouping<Tuple<int, string?>, ExternalXmlPropertyDisplayData>? externalGrouping
                = externalPropertiesGrouped.FirstOrDefault(x => x.Key.Equals(localGrouping.Key));

            if (excludeLocalCharacteristicsWithNoMatches)
            {
                bool characteristicHasNoMatchingExternalPropertiesInRelationData = relationToAddDataTo is null;
                bool characteristicHasNoMatchingExternalPropertiesInGroupings = (externalGrouping is null || !externalGrouping.Any());

                if (characteristicHasNoMatchingExternalPropertiesInRelationData && characteristicHasNoMatchingExternalPropertiesInGroupings) continue;
            }

            if (relationToAddDataTo is null)
            {
                relationToAddDataTo = new();

                output.Add(relationToAddDataTo);
            }

            relationToAddDataTo.AddCharacteristics(localGrouping);

            if (externalGrouping is not null)
            {
                relationToAddDataTo.AddProperties(externalGrouping);

                externalPropertiesGrouped.Remove(externalGrouping);
            }
        }

        if (!excludeLocalCharacteristicsWithNoMatches)
        {
            foreach (IGrouping<Tuple<int, string?>, ExternalXmlPropertyDisplayData> externalGrouping in externalPropertiesGrouped)
            {
                ExternalAndLocalCharacteristicRelationDisplayData item = new();

                item.AddProperties(externalGrouping);

                output.Add(item);
            }
        }

        return output
            .OrderBy(x => x.MatchingExternalProperties?.ElementAtOrDefault(0)?.CategoryId ?? 0)
            .ThenBy(x => x.MatchingExternalProperties?.ElementAtOrDefault(0)?.Name ?? string.Empty)
            .ToList();
    }

    private static bool IsCharacteristicDataAlreadyAdded(LocalCharacteristicDisplayData localCharacteristicDisplayData,
        List<ExternalAndLocalCharacteristicRelationDisplayData> addedRelations)
    {
        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in addedRelations)
        {
            if (relationDisplayData.MatchingLocalCharacteristics is null
                || relationDisplayData.MatchingLocalCharacteristics.Count <= 0) continue;

            LocalCharacteristicDisplayData? matchingCharacteristicInRelation = relationDisplayData.MatchingLocalCharacteristics?.FirstOrDefault(x =>
                ExternalAndLocalCharacteristicRelationDisplayData.IsValueEqual(localCharacteristicDisplayData, x));

            if (matchingCharacteristicInRelation is not null) return true;
        }

        return false;
    }

    private static bool IsExternalXmlPropertyDataAlreadyAdded(ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData,
        List<ExternalAndLocalCharacteristicRelationDisplayData> addedRelations)
    {
        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in addedRelations)
        {
            if (relationDisplayData.MatchingExternalProperties is null
                || relationDisplayData.MatchingExternalProperties.Count <= 0) continue;

            ExternalXmlPropertyDisplayData? matchingExternalXmlPropertyInRelation = relationDisplayData.MatchingExternalProperties?.FirstOrDefault(x =>
                ExternalAndLocalCharacteristicRelationDisplayData.IsValueEqual(externalXmlPropertyDisplayData, x));

            if (matchingExternalXmlPropertyInRelation is not null) return true;
        }

        return false;
    }

    private List<ExternalAndLocalCharacteristicRelationDisplayData> GetRelationshipDisplayDataFromRelationData(
        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> productCharacteristicAndExternalXmlPropertyRelations)
    {
        List<ExternalAndLocalCharacteristicRelationDisplayData> output = new();

        IEnumerable<int> productCharacteristicIds = productCharacteristicAndExternalXmlPropertyRelations
            .Select(x => x.ProductCharacteristicId)
            .Where(id => id is not null)
            .Cast<int>();

        IEnumerable<ProductCharacteristic> characteristicsForRelations
            = _productCharacteristicService.GetSelectionByCharacteristicIds(productCharacteristicIds);

        foreach (ProductCharacteristicAndExternalXmlDataRelation characteristicAndExternalXmlDataRelation in productCharacteristicAndExternalXmlPropertyRelations)
        {
            if (characteristicAndExternalXmlDataRelation.ProductCharacteristicId is null) continue;

            ProductCharacteristic? matchingCharacteristic = characteristicsForRelations.FirstOrDefault(
                x => x.Id == characteristicAndExternalXmlDataRelation.ProductCharacteristicId);

            LocalCharacteristicDisplayData localCharacteristicData = new()
            {
                Id = characteristicAndExternalXmlDataRelation.ProductCharacteristicId,
                CategoryId = characteristicAndExternalXmlDataRelation.CategoryId,
                Name = characteristicAndExternalXmlDataRelation.ProductCharacteristicName,
                Meaning = characteristicAndExternalXmlDataRelation.ProductCharacteristicMeaning,
                DisplayOrder = matchingCharacteristic?.DisplayOrder,
            };

            ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData = new()
            {
                Name = characteristicAndExternalXmlDataRelation.XmlName,
                Order = characteristicAndExternalXmlDataRelation.XmlDisplayOrder,
                CategoryId = characteristicAndExternalXmlDataRelation.CategoryId,
                Value = null,
            };

            ExternalAndLocalCharacteristicRelationDisplayData? relationToAddDataTo
                = output.FirstOrDefault(x => localCharacteristicData.Name == IsCharacteristicMatchedWithRelationshipByName(x));

            if (relationToAddDataTo is null)
            {
                relationToAddDataTo = new();

                output.Add(relationToAddDataTo);
            }

            relationToAddDataTo.AddCharacteristics(localCharacteristicData);

            relationToAddDataTo.AddProperties(externalXmlPropertyDisplayData);

            relationToAddDataTo.MatchCharacteristicAndProperty(localCharacteristicData, externalXmlPropertyDisplayData, Color.Aqua);
        }

        return output;
    }

    private static string? IsCharacteristicMatchedWithRelationshipByName(ExternalAndLocalCharacteristicRelationDisplayData localCharacteristicRelationDisplayData)
    {
        if (localCharacteristicRelationDisplayData.MatchingExternalProperties?.Count > 0)
        {
            return localCharacteristicRelationDisplayData.MatchingExternalProperties[0].Name;
        }

        else if (localCharacteristicRelationDisplayData.MatchingLocalCharacteristics?.Count > 0)
        {
            return localCharacteristicRelationDisplayData.MatchingLocalCharacteristics[0].Name;
        }
        return null;
    }

    private Task<string> GetLocalCharacteristicsPartialViewAsString(IEnumerable<LocalCharacteristicDisplayData> characteristics)
    {
        return this.RenderPartialViewToStringAsync(
            "ProductCharacteristicsComparer/_LocalCharacteristicsListPartial",
            new LocalCharacteristicsListPartialModel()
            {
                Characteristics = characteristics,
                ExternalAndLocalCharacteristicRelations = ExternalAndLocalCharacteristicRelations
            },
            _razorViewEngine,
            _tempDataProvider);
    }

    private Task<string> GetExternalPropertiesPartialViewAsString(IEnumerable<ExternalXmlPropertyDisplayData> properties)
    {
        return this.RenderPartialViewToStringAsync(
            "ProductCharacteristicsComparer/_ExternalPropertiesListPartial",
            new ExternalPropertiesListPartialModel()
            {
                Properties = properties,
                ExternalAndLocalCharacteristicRelations = ExternalAndLocalCharacteristicRelations
            },
            _razorViewEngine,
            _tempDataProvider);
    }

    private Task<string> GetCharacteristicsRelationshipTablePartialViewAsString(
        List<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        return this.RenderPartialViewToStringAsync(
            "ProductCharacteristicsComparer/_CharacteristicRelationshipTablePartial",
            new CharacteristicRelationshipTablePartialModel()
            {
                ExternalAndLocalCharacteristicRelations = externalAndLocalCharacteristicRelations,
                CategoriesForItems = GetRelevantCategories(externalAndLocalCharacteristicRelations),
                RelationshipTableViewContainerId = "characteristics_comparer_relationships_table_view_container",
                RelationshipXmlViewContainerId = "characteristics_comparer_relationships_table_xml_view_container",
            },
            _razorViewEngine,
            _tempDataProvider);
    }

    private PartialViewResult GetCharacteristicsRelationshipTablePartialView(
        List<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        return Partial(
            "ProductCharacteristicsComparer/_CharacteristicRelationshipTablePartial",
            new CharacteristicRelationshipTablePartialModel()
            {
                ExternalAndLocalCharacteristicRelations = externalAndLocalCharacteristicRelations,
                CategoriesForItems = GetRelevantCategories(externalAndLocalCharacteristicRelations),
                RelationshipTableViewContainerId = "characteristics_comparer_relationships_table_view_container",
                RelationshipXmlViewContainerId = "characteristics_comparer_relationships_table_xml_view_container",
            });
    }

    private PartialViewResult GetLocalAndExternalXmlTablePartialView(string? localXml, string? externalXml)
    {
        return Partial(
            "ProductCharacteristicsComparer/_LocalAndExternalXmlComparerPartial",
            new LocalAndExternalXmlComparerPartialModel()
            {
                LocalXml = localXml,
                ExternalXml = externalXml
            });
    }

    private List<Category> GetRelevantCategories(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        List<Category> allCategories = GetAllCategories();

        List<Category> categories = new();

        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in externalAndLocalCharacteristicRelations)
        {
            if (relationDisplayData.MatchingLocalCharacteristics?.Count > 0)
            {
                foreach (LocalCharacteristicDisplayData localCharacteristic in relationDisplayData.MatchingLocalCharacteristics)
                {
                    Category? matchingCategory = categories.FirstOrDefault(category => category.Id == localCharacteristic.CategoryId);

                    if (matchingCategory is not null) continue;
                    
                    Category? category = allCategories.FirstOrDefault(category => category.Id == localCharacteristic.CategoryId);

                    if (category is null) continue;

                    categories.Add(category);
                }
            }

            if (relationDisplayData.MatchingExternalProperties?.Count > 0)
            {
                foreach (ExternalXmlPropertyDisplayData externalXmlProperty in relationDisplayData.MatchingExternalProperties)
                {
                    Category? matchingCategory = categories.FirstOrDefault(category => category.Id == externalXmlProperty.CategoryId);

                    if (matchingCategory is not null) continue;
                    
                    Category? category = allCategories.FirstOrDefault(category => category.Id == externalXmlProperty.CategoryId);

                    if (category is null) continue;
                    
                    categories.Add(category);
                }
            }
        }

        return categories;
    }

    private static ProductImage Map(XmlImportProductImage xmlImportProductImage)
    {
        return new()
        {
            Id = xmlImportProductImage.Id,
            ProductId = xmlImportProductImage.ProductId,
            ImageContentType = xmlImportProductImage.ImageContentType,
            ImageData = xmlImportProductImage.ImageData,
            HtmlData = xmlImportProductImage.HtmlData,
            DateModified = xmlImportProductImage.DateModified,
        };
    }

    private static ProductImageFileNameInfo Map(XmlImportProductImageFileNameInfo xmlImportProductImageFileNameInfo)
    {
        return new()
        {
            ProductId = xmlImportProductImageFileNameInfo.ProductId,
            ImageNumber = xmlImportProductImageFileNameInfo.ImageNumber,
            FileName = xmlImportProductImageFileNameInfo.FileName,
            DisplayOrder = xmlImportProductImageFileNameInfo.DisplayOrder,
            Active = xmlImportProductImageFileNameInfo.Active,
        };
    }

    private static ProductProperty Map(XmlImportProductProperty xmlImportProductProperty)
    {
        return new()
        {
            ProductCharacteristicId = xmlImportProductProperty.ProductCharacteristicId,
            ProductId = xmlImportProductProperty.ProductId,
            Characteristic = xmlImportProductProperty.Characteristic,
            DisplayOrder = xmlImportProductProperty.DisplayOrder,
            Value = xmlImportProductProperty.Value,
            XmlPlacement = xmlImportProductProperty.XmlPlacement,
        };
    }

    private class ExternalXmlPropertyDisplayDataEqualityComparerWithoutValueComparison : EqualityComparer<ExternalXmlPropertyDisplayData>
    {
        public override bool Equals(ExternalXmlPropertyDisplayData? x, ExternalXmlPropertyDisplayData? y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null) return false;

            return x.CategoryId == y.CategoryId
                && x.Name == y.Name
                && x.Order == y.Order;
        }

        public override int GetHashCode([DisallowNull] ExternalXmlPropertyDisplayData obj)
        {
            return obj.Order ?? -1;
        }
    }
}

public class BackgroundColorDTO
{
    public int Red { get; set; }
    public int Blue { get; set; }
    public int Green { get; set; }
}