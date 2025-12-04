using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.UI.Web.Models.Configuration;
using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;
using MOSTComputers.UI.Web.Pages.Shared.ProductCharacteristicsComparer;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.Services.Data.PageDataStorage.Contracts;
using MOSTComputers.UI.Web.Services.Data.Xml.Contracts;
using MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;
using OneOf;
using OneOf.Types;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection.PortableExecutable;
using static MOSTComputers.UI.Web.Utils.Mapping.ProductCharacteristicComparerMappingUtils;
using static MOSTComputers.Utils.OneOf.AsyncMatchingExtensions;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public class ProductCharacteristicsComparerModel : PageModel
{
    public ProductCharacteristicsComparerModel(
        IOptions<LegacyPricelistSiteOptions> legacyPricelistSiteOptions,
        IProductCharacteristicRelationsStorageService productCharacteristicRelationsStorageService,
        IProductXmlProvidingService productXmlProvidingService,
        ICategoryService categoryService,
        IProductService productService,
        IProductImageService productImageService,
        IProductCharacteristicAndExternalXmlDataRelationService productCharacteristicAndExternalXmlDataRelationService,
        IProductPropertyService productPropertyService,
        IProductImageFileService productImageFileService,
        IProductCharacteristicService productCharacteristicService,
        ILegacyProductXmlService productDeserializeService,
        ILegacyProductXmlValidationService legacyProductXmlValidationService,
        ILegacyProductToXmlProductMappingService productToXmlProductMappingService,
        ILegacyProductXmlFromProductDataService legacyProductXmlFromProductDataService,
        IPartialViewRenderService partialViewRenderService,
        ITransactionExecuteService transactionExecuteService)
    {
        _legacyPricelistSiteOptions = legacyPricelistSiteOptions;
        _productCharacteristicRelationsStorageService = productCharacteristicRelationsStorageService;
        _productXmlProvidingService = productXmlProvidingService;
        _categoryService = categoryService;
        _productService = productService;
        _productImageService = productImageService;
        _productCharacteristicAndExternalXmlDataRelationService = productCharacteristicAndExternalXmlDataRelationService;
        _productPropertyService = productPropertyService;
        _productImageFileService = productImageFileService;
        _productCharacteristicService = productCharacteristicService;
        _productDeserializeService = productDeserializeService;
        _legacyProductXmlValidationService = legacyProductXmlValidationService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
        _legacyProductXmlFromProductDataService = legacyProductXmlFromProductDataService;
        _partialViewRenderService = partialViewRenderService;
        _transactionExecuteService = transactionExecuteService;
    }

    private const string _manufacturerCharacteristicName = "Manufacturer";

    public const string TopNotificationBoxElementId = "topNotificationBox";
    private readonly IOptions<LegacyPricelistSiteOptions> _legacyPricelistSiteOptions;
    private readonly IProductCharacteristicRelationsStorageService _productCharacteristicRelationsStorageService;

    private readonly IProductXmlProvidingService _productXmlProvidingService;

    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductCharacteristicAndExternalXmlDataRelationService _productCharacteristicAndExternalXmlDataRelationService;

    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageFileService _productImageFileService;

    private readonly IProductCharacteristicService _productCharacteristicService;

    private readonly ILegacyProductXmlService _productDeserializeService;
    private readonly ILegacyProductXmlValidationService _legacyProductXmlValidationService;
    private readonly ILegacyProductToXmlProductMappingService _productToXmlProductMappingService;
    private readonly ILegacyProductXmlFromProductDataService _legacyProductXmlFromProductDataService;
    private readonly IPartialViewRenderService _partialViewRenderService;

    private readonly ITransactionExecuteService _transactionExecuteService;

    public async Task<IActionResult> OnPostGetLocalAndExternalXmlDataForCategoryAsync(int categoryId)
    {
        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlResult.MatchAsync(
            async productXmlData =>
            {
                if (productXmlData?.Products is not null
                    && productXmlData.Products.Count > 0)
                {
                    productXmlData.Products = productXmlData.Products.FindAll(x => x.Category?.Id == categoryId);

                    return await GetLocalAndExternalXmlDataForCategoryAsync(categoryId, productXmlData.Products);
                }

                return new OkResult();
            },
            invalidXmlResult => BadRequest(invalidXmlResult),
            notFound => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<IActionResult> GetLocalAndExternalXmlDataForCategoryAsync(int categoryId, List<LegacyXmlProduct> xmlProducts)
    {
        List<Product> productsInCategory = await _productService.GetAllInCategoryAsync(categoryId);

        IEnumerable<Product> productsFromSameCategory = productsInCategory
            .OrderByDescending(product => xmlProducts.FirstOrDefault(
                xmlProduct => xmlProduct.Id == product.Id) is not null)
            .ThenBy(product => product.Id);

        xmlProducts = xmlProducts.OrderByDescending(
            xmlProduct => productsFromSameCategory.FirstOrDefault(
                product => product.Id == xmlProduct.Id) is not null)
            .ThenBy(xmlProduct => xmlProduct.Id)
            .ToList();

        string legacyProductsXmlGetImageEndpointPath = _legacyPricelistSiteOptions.Value.LegacyProductsXmlGetImageEndpointPath;

        LegacyXmlObjectData xmlObjectData = await _legacyProductXmlFromProductDataService.GetLegacyXmlObjectDataForProductsAsync(
            productsFromSameCategory.ToList(),
            legacyProductsXmlGetImageEndpointPath);

        for (int i = 0; i < xmlObjectData.Products.Count; i++)
        {
            LegacyXmlProduct xmlProduct = xmlObjectData.Products[i];

            if (_legacyProductXmlValidationService.IsValidXmlProduct(xmlProduct)) continue;

            xmlObjectData.Products.RemoveAt(i);

            i--;
        }

        OneOf<string, InvalidXmlResult> localXmlProductsSerializeResult = _productDeserializeService.TrySerializeProductsXml(xmlObjectData, true);

        return localXmlProductsSerializeResult.Match(
            localXml =>
            {
                LegacyXmlObjectData localXmlObjectData = new()
                {
                    Products = xmlProducts
                };

                for (int i = 0; i < localXmlObjectData.Products.Count; i++)
                {
                    LegacyXmlProduct xmlProduct = localXmlObjectData.Products[i];

                    if (_legacyProductXmlValidationService.IsValidXmlProduct(xmlProduct)) continue;

                    localXmlObjectData.Products.RemoveAt(i);

                    i--;
                }

                OneOf<string, InvalidXmlResult> externalXmlProductsSerializeResult
                    = _productDeserializeService.TrySerializeProductsXml(localXmlObjectData, true);

                return externalXmlProductsSerializeResult.Match<IStatusCodeActionResult>(
                    externalXml => GetLocalAndExternalXmlTablePartialView(localXml, externalXml),
                    invalidXmlResult => BadRequest(invalidXmlResult));
            },
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    private async Task<IStatusCodeActionResult> CompareExternalAndLocalCharacteristicsForCategoriesAsync(IEnumerable<int> categoryIds)
    {
        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlResult.MatchAsync(
            async productXml =>
            {
                List<LegacyXmlProduct>? products = productXml.Products
                    .Where(x => categoryIds.Contains(x.Category.Id))
                    .ToList();

                List<ProductCharacteristicAndExternalXmlDataRelation>? existingRelations = await GetExistingRelationsForProductsAsync(products);

                OneOf<List<ExternalAndLocalCharacteristicRelationDisplayData>, IStatusCodeActionResult> compareResult = await CompareExternalAndLocalDataAsync(
                    products,
                    alreadyAddedRelations: existingRelations,
                    excludeLocalCharacteristicsWithNoMatches: false,
                    addBaseCategoryCharacteristics: false);

                return await compareResult.MatchAsync(
                    async relations =>
                    {
                        _productCharacteristicRelationsStorageService.Clear();
                        _productCharacteristicRelationsStorageService.AddRange(relations);

                        return await GetAllPartialViewsResultAsync(relations);
                    },
                    statusCodeResult => Task.FromResult(statusCodeResult));
            },
            invalidXmlResult => StatusCode(StatusCodes.Status500InternalServerError),
            notFound => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<List<ProductCharacteristicAndExternalXmlDataRelation>?> GetExistingRelationsForProductsAsync(List<LegacyXmlProduct>? products)
    {
        if (products is null) return null;

        List<ProductCharacteristicAndExternalXmlDataRelation>? existingRelations = null;

        IEnumerable<int>? categoriesInXml = products
            .Select(x => x.Category.Id)
            .Distinct();

        if (categoriesInXml is not null)
        {
            existingRelations = await _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdsAsync(categoriesInXml);
        }

        return existingRelations;
    }

    private async Task<IStatusCodeActionResult> GetAllPartialViewsResultAsync(List<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        Task<string> localDataPartialViewTask = GetLocalCharacteristicsPartialViewAsString(relations);
        Task<string> externalDataPartialViewTask = GetExternalPropertiesPartialViewAsString(relations);
        Task<string> characteristicsRelationshipTablePartialViewTask = GetCharacteristicsRelationshipTablePartialViewAsStringAsync(relations);

        return new JsonResult(new
        {
            localCharacteristicsPartialViewText = await localDataPartialViewTask,
            externalPropertiesPartialViewText = await externalDataPartialViewTask,
            characteristicsRelationshipTablePartialViewText = await characteristicsRelationshipTablePartialViewTask
        });
    }

    public async Task<IActionResult> OnPostCompareExternalAndLocalDataForCategoryAsync(int categoryId)
    {
        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlResult.MatchAsync(
            async productXml =>
            {
                productXml.Products = productXml.Products.FindAll(x => x.Category?.Id == categoryId);

                List<ProductCharacteristicAndExternalXmlDataRelation> alreadyAddedRelations
                    = await _productCharacteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdAsync(categoryId);

                OneOf<List<ExternalAndLocalCharacteristicRelationDisplayData>, IStatusCodeActionResult> compareResult = await CompareExternalAndLocalDataAsync(
                    productXml.Products,
                    excludeLocalCharacteristicsWithNoMatches: false,
                    addBaseCategoryCharacteristics: false,
                    alreadyAddedRelations: alreadyAddedRelations);

                return await compareResult.Match(
                    relations =>
                    {
                        _productCharacteristicRelationsStorageService.Clear();
                        _productCharacteristicRelationsStorageService.AddRange(relations);

                        return GetAllPartialViewsResultAsync(relations);
                    },
                    statusCodeResult => Task.FromResult(statusCodeResult));
            },
            invalidXmlResult => StatusCode(StatusCodes.Status500InternalServerError),
            notFound => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPostSaveAllRelationshipsAsync()
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            SaveAllRelationshipsInternalAsync,
            result => result is JsonResult || result.StatusCode == 200);
    }

    private async Task<IStatusCodeActionResult> SaveAllRelationshipsInternalAsync()
    {
        IReadOnlyList<ExternalAndLocalCharacteristicRelationDisplayData> relationsToSave
            = _productCharacteristicRelationsStorageService.ProductCharacteristicRelations;

        List<Category> allCategories = await GetAllCategoriesAsync();

        IEnumerable<int> matchedCategoryIds = relationsToSave
            .Where(x => x.MatchingLocalCharacteristic?.CategoryId is not null)
            .Select(x => (int)x.MatchingLocalCharacteristic!.CategoryId!)
            .Distinct();

        foreach (int categoryId in matchedCategoryIds)
        {
            await _productCharacteristicAndExternalXmlDataRelationService.DeleteAllWithSameCategoryIdAsync(categoryId);
        }

        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in relationsToSave)
        {
            if (relationDisplayData.MatchingLocalCharacteristic is null
                || relationDisplayData.MatchingExternalProperties is null
                || relationDisplayData.MatchingExternalProperties.Count <= 0)
            {
                continue;
            }

            LocalCharacteristicDisplayData characteristicDataInRelation = relationDisplayData.MatchingLocalCharacteristic;

            Category? matchingCategory = allCategories.FirstOrDefault(x => x.Id == characteristicDataInRelation.CategoryId);

            if (matchingCategory is null) return BadRequest();

            foreach (ExternalXmlPropertyDisplayData externalXmlProperty in relationDisplayData.MatchingExternalProperties)
            {
                ProductCharacteristicAndExternalXmlDataRelationUpsertRequest productCharacteristicAndExternalXmlDataRelationUpsertRequest = new()
                {
                    CategoryId = externalXmlProperty.CategoryId,
                    ProductCharacteristicId = characteristicDataInRelation.Id,
                    XmlName = externalXmlProperty.Name,
                    XmlDisplayOrder = externalXmlProperty.Order
                };

                OneOf<Success, UnexpectedFailureResult> upsertRelationResult
                    = await _productCharacteristicAndExternalXmlDataRelationService.UpsertByCharacteristicIdAsync(productCharacteristicAndExternalXmlDataRelationUpsertRequest);

                if (!upsertRelationResult.IsT0) return new StatusCodeResult(500);
            }
        }

        return await CompareExternalAndLocalCharacteristicsForCategoriesAsync(matchedCategoryIds);
    }

    public async Task<IActionResult> OnPutMoveItemsFromOneRelationshipToAnotherAsync(
        int indexOfRelationshipToMoveFrom,
        int indexOfRelationshipToMoveTo,
        LocalCharacteristicOrExternalPropertyEnum moveCharacteristicsOrProperties,
        [FromBody] int[] indexesOfItemsToMove)
    {
        if (indexesOfItemsToMove.Length == 0) return BadRequest();

        IReadOnlyList<ExternalAndLocalCharacteristicRelationDisplayData> allRelations = _productCharacteristicRelationsStorageService.ProductCharacteristicRelations;

        if (indexOfRelationshipToMoveFrom < 0
            || indexOfRelationshipToMoveFrom >= allRelations.Count
            || indexOfRelationshipToMoveTo < 0
            || indexOfRelationshipToMoveTo >= allRelations.Count)
        {
            return BadRequest();
        }

        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveFrom = allRelations[indexOfRelationshipToMoveFrom];
        ExternalAndLocalCharacteristicRelationDisplayData relationshipToMoveTo = allRelations[indexOfRelationshipToMoveTo];

        if (moveCharacteristicsOrProperties == LocalCharacteristicOrExternalPropertyEnum.Characteristic)
        {
            if (relationshipToMoveFrom.MatchingLocalCharacteristic is null)
            {
                return (indexesOfItemsToMove.Length == 0) ? new OkResult() : BadRequest();
            }

            if (relationshipToMoveFrom.MatchingLocalCharacteristic is null
                || relationshipToMoveTo.MatchingLocalCharacteristic is not null)
            {
                return BadRequest();
            }

            LocalCharacteristicDisplayData characteristicToMove = relationshipToMoveFrom.MatchingLocalCharacteristic;

            relationshipToMoveFrom.RemoveCharacteristic();

            relationshipToMoveTo.AddCharacteristic(characteristicToMove);

            return await GetCharacteristicsRelationshipTablePartialViewAsync(allRelations.ToList());
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

        return await GetCharacteristicsRelationshipTablePartialViewAsync(allRelations.ToList());
    }

    public async Task<IActionResult> OnDeleteDeleteSavedRelationsForCategoryAsync(int categoryId)
    {
        if (categoryId < -1) return BadRequest();

        bool isDeleteSuccessful = await _productCharacteristicAndExternalXmlDataRelationService.DeleteAllWithSameCategoryIdAsync(categoryId);

        if (!isDeleteSuccessful) return NotFound();

        return await OnPostCompareExternalAndLocalDataForCategoryAsync(categoryId);
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _categoryService.GetAllAsync();
    }

    private async Task<List<ExternalAndLocalCharacteristicRelationDisplayData>> CompareExternalAndLocalDataAsync(
        List<LegacyXmlProduct> xmlProducts,
        bool excludeLocalCharacteristicsWithNoMatches,
        bool addBaseCategoryCharacteristics,
        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation>? alreadyAddedRelations = null)
    {
        List<ExternalXmlPropertyDisplayData> externalPropsToCompare = GetExternalDistinctProperties(xmlProducts);

        List<int> categoryIds = new();

        if (addBaseCategoryCharacteristics)
        {
            categoryIds.Add(-1);
        }

        foreach (LegacyXmlProduct xmlProduct in xmlProducts)
        {
            int categoryId = xmlProduct.Category.Id;

            if (!categoryIds.Contains(categoryId))
            {
                categoryIds.Add(categoryId);
            }
        }

        List<LocalCharacteristicDisplayData> localDistinctCharacteristics = await GetLocalCharacteristicDisplayDataAsync(
            categoryIds, externalPropsToCompare, excludeLocalCharacteristicsWithNoMatches);

        List<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations = GetCharacteristicRelations(
            localDistinctCharacteristics,
            externalPropsToCompare,
            excludeLocalCharacteristicsWithNoMatches,
            alreadyAddedRelations);

        return externalAndLocalCharacteristicRelations;
    }

    private async Task<List<LocalCharacteristicDisplayData>> GetLocalCharacteristicDisplayDataAsync(
        List<int> categoryIds,
        IEnumerable<ExternalXmlPropertyDisplayData> externalProperties,
        bool excludeLocalCharacteristicsWithNoMatches)
    {
        List<ProductCharacteristic> characteristicsForAllRelevantCategories
            = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(categoryIds, [ProductCharacteristicType.ProductCharacteristic]);

        if (characteristicsForAllRelevantCategories.Count <= 0) return new();

        IEnumerable<ProductCharacteristic> matchingCharacteristics;

        if (!excludeLocalCharacteristicsWithNoMatches)
        {
            matchingCharacteristics = characteristicsForAllRelevantCategories
                .Where(characteristic => characteristic.Active == true)
                .OrderBy(characteristic => characteristic.CategoryId)
                .ThenBy(characteristic => characteristic.Name)
                .ThenBy(characteristic => characteristic.DisplayOrder);

            return MapManyToLocalCharacteristicDisplayData(matchingCharacteristics);
        }

        matchingCharacteristics = characteristicsForAllRelevantCategories
            .Where(characteristic =>
            {
                if (characteristic.Active != true) return false;

                ExternalXmlPropertyDisplayData? matchingPropertyData = externalProperties.FirstOrDefault(externalProperty =>
                    externalProperty.CategoryId == characteristic.CategoryId
                        && externalProperty.Name == characteristic.Name);

                return matchingPropertyData is not null;
            })
            .OrderBy(characteristic => characteristic.CategoryId)
            .ThenBy(characteristic => characteristic.Name)
            .ThenBy(characteristic => characteristic.DisplayOrder);

        return MapManyToLocalCharacteristicDisplayData(matchingCharacteristics);
    }

    private static List<ExternalXmlPropertyDisplayData> GetExternalDistinctProperties(IEnumerable<LegacyXmlProduct> xmlProducts)
    {
        List<ExternalXmlPropertyDisplayData> output = new();

        foreach (LegacyXmlProduct xmlProduct in xmlProducts)
        {
            foreach (LegacyXmlProductProperty xmlProperty in xmlProduct.Properties)
            {
                ExternalXmlPropertyDisplayData externalPropertyData = MapToExternalXmlPropertyDisplayData(xmlProduct.Category.Id, xmlProperty);

                output.Add(externalPropertyData);
            }
        }

        output = output
            .Where(x => x.Name != _manufacturerCharacteristicName)
            .Distinct(new ExternalXmlPropertyDisplayDataEqualityComparerWithoutValueComparison())
            .OrderBy(x => x.CategoryId)
            .ThenBy(x => x.Order)
            .ToList();

        return output;
    }

    private static List<ExternalAndLocalCharacteristicRelationDisplayData> GetCharacteristicRelations(
        List<LocalCharacteristicDisplayData> localDistinctCharacteristics,
        List<ExternalXmlPropertyDisplayData> externalDistinctProperties,
        bool excludeLocalCharacteristicsWithNoMatches,
        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation>? productCharacteristicAndExternalXmlPropertyRelations = null)
    {
        List<ExternalAndLocalCharacteristicRelationDisplayData> output = new();

        Dictionary<Tuple<int, string?>, List<ExternalXmlPropertyDisplayData>> externalPropertiesGrouped
            = GroupExternalProperties(externalDistinctProperties);

        foreach (LocalCharacteristicDisplayData localCharacteristic in localDistinctCharacteristics)
        {
            ExternalAndLocalCharacteristicRelationDisplayData relationToAddDataTo = new();

            output.Add(relationToAddDataTo);

            relationToAddDataTo.AddCharacteristic(localCharacteristic);

            IEnumerable<ProductCharacteristicAndExternalXmlDataRelation>? matchingRelations
                = productCharacteristicAndExternalXmlPropertyRelations?.Where(
                    x => x.ProductCharacteristicId == localCharacteristic.Id);

            if (matchingRelations is null) continue;

            foreach (ProductCharacteristicAndExternalXmlDataRelation relation in matchingRelations)
            {
                bool propertiesWithSameNameExist = externalPropertiesGrouped
                    .TryGetValue(new(relation.CategoryId, relation.XmlName), out List<ExternalXmlPropertyDisplayData>? matchingExternalProperties);

                if (propertiesWithSameNameExist)
                {
                    int externalPropertyForRelationIndex = matchingExternalProperties!
                        .FindIndex(x => x.Order == relation.XmlDisplayOrder);

                    if (externalPropertyForRelationIndex >= 0)
                    {
                        relationToAddDataTo.AddProperties(matchingExternalProperties[externalPropertyForRelationIndex]);

                        matchingExternalProperties.RemoveAt(externalPropertyForRelationIndex);
                    }
                    else
                    {
                        ExternalXmlPropertyDisplayData missingExternalPropertyMarkedInRelations = new()
                        {
                            CategoryId = relation.CategoryId,
                            Name = relation.XmlName,
                            Order = relation.XmlDisplayOrder,
                            CustomBackgroundColor = Color.Yellow,
                        };

                        relationToAddDataTo.AddProperties(missingExternalPropertyMarkedInRelations);
                    }
                }
            }
        }

        if (!excludeLocalCharacteristicsWithNoMatches)
        {
            foreach (KeyValuePair<Tuple<int, string?>, List<ExternalXmlPropertyDisplayData>> externalGrouping in externalPropertiesGrouped)
            {
                if (externalGrouping.Value.Count <= 0) continue;

                ExternalAndLocalCharacteristicRelationDisplayData item = new();

                item.AddProperties(externalGrouping.Value);

                output.Add(item);
            }
        }

        return output
            .OrderBy(x => x.MatchingLocalCharacteristic?.DisplayOrder ?? int.MaxValue)
            .ToList();
    }

    private static Dictionary<Tuple<int, string?>, List<ExternalXmlPropertyDisplayData>> GroupExternalProperties(
        List<ExternalXmlPropertyDisplayData> externalDistinctProperties)
    {
        Dictionary<Tuple<int, string?>, List<ExternalXmlPropertyDisplayData>> output = new();

        foreach (ExternalXmlPropertyDisplayData externalDistinctProperty in externalDistinctProperties)
        {
            Tuple<int, string?> key = new(externalDistinctProperty.CategoryId, externalDistinctProperty.Name);

            bool success = output.TryGetValue(key, out List<ExternalXmlPropertyDisplayData>? matchingExternalProperties);

            if (success)
            {
                matchingExternalProperties!.Add(externalDistinctProperty);

                continue;
            }

            output.Add(key, new() { externalDistinctProperty });
        }

        return output;
    }

    private Task<string> GetLocalCharacteristicsPartialViewAsString(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        LocalCharacteristicsListPartialModel localCharacteristicsPartialModel = GetLocalCharacteristicsPartialModel(relations);

        return _partialViewRenderService.RenderPartialViewToStringAsync(this,
            "ProductCharacteristicsComparer/_LocalCharacteristicsListPartial",
            localCharacteristicsPartialModel);
    }

    public LocalCharacteristicsListPartialModel GetLocalCharacteristicsPartialModelWithCurrentData()
    {
        return GetLocalCharacteristicsPartialModel(_productCharacteristicRelationsStorageService.ProductCharacteristicRelations);
    }

    private static LocalCharacteristicsListPartialModel GetLocalCharacteristicsPartialModel(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        return new LocalCharacteristicsListPartialModel()
        {
            Characteristics = relations
                .Select(x => x.MatchingLocalCharacteristic)
                .Where(x => x is not null)!,

            ExternalAndLocalCharacteristicRelations = relations
        };
    }

    private Task<string> GetExternalPropertiesPartialViewAsString(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        ExternalPropertiesListPartialModel externalPropertiesPartialModel = GetExternalPropertiesPartialModel(relations);

        return _partialViewRenderService.RenderPartialViewToStringAsync(this,
            "ProductCharacteristicsComparer/_ExternalPropertiesListPartial",
            externalPropertiesPartialModel);
    }

    public ExternalPropertiesListPartialModel GetExternalPropertiesPartialModelWithCurrentData()
    {
        return GetExternalPropertiesPartialModel(_productCharacteristicRelationsStorageService.ProductCharacteristicRelations);
    }

    private static ExternalPropertiesListPartialModel GetExternalPropertiesPartialModel(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        return new ExternalPropertiesListPartialModel()
        {
            Properties = relations
                .Where(x => x.MatchingExternalProperties is not null)
                .SelectMany(x => x.MatchingExternalProperties!),

            ExternalAndLocalCharacteristicRelations = relations
        };
    }

    private async Task<string> GetCharacteristicsRelationshipTablePartialViewAsStringAsync(
        List<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        return await _partialViewRenderService.RenderPartialViewToStringAsync(
            this,
            "ProductCharacteristicsComparer/_CharacteristicRelationshipTablePartial",
            new CharacteristicRelationshipTablePartialModel()
            {
                ExternalAndLocalCharacteristicRelations = externalAndLocalCharacteristicRelations,
                CategoriesForItems = await GetRelevantCategoriesAsync(externalAndLocalCharacteristicRelations),
                RelationshipTableViewContainerId = "characteristics_comparer_relationships_table_view_container",
                RelationshipXmlViewContainerId = "characteristics_comparer_relationships_table_xml_view_container",
                NotificationBoxElementId = TopNotificationBoxElementId,
            });
    }

    private async Task<PartialViewResult> GetCharacteristicsRelationshipTablePartialViewAsync(
        List<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        return Partial(
            "ProductCharacteristicsComparer/_CharacteristicRelationshipTablePartial",
            new CharacteristicRelationshipTablePartialModel()
            {
                ExternalAndLocalCharacteristicRelations = externalAndLocalCharacteristicRelations,
                CategoriesForItems = await GetRelevantCategoriesAsync(externalAndLocalCharacteristicRelations),
                RelationshipTableViewContainerId = "characteristics_comparer_relationships_table_view_container",
                RelationshipXmlViewContainerId = "characteristics_comparer_relationships_table_xml_view_container",
                NotificationBoxElementId = TopNotificationBoxElementId,
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

    private async Task<List<Category>> GetRelevantCategoriesAsync(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> externalAndLocalCharacteristicRelations)
    {
        List<Category> allCategories = await GetAllCategoriesAsync();

        List<Category> categories = new();

        foreach (ExternalAndLocalCharacteristicRelationDisplayData relationDisplayData in externalAndLocalCharacteristicRelations)
        {
            if (relationDisplayData.MatchingLocalCharacteristic is not null)
            {
                Category? matchingCategory = categories.FirstOrDefault(category => category.Id == relationDisplayData.MatchingLocalCharacteristic.CategoryId);

                if (matchingCategory is not null) continue;

                Category? category = allCategories.FirstOrDefault(category => category.Id == relationDisplayData.MatchingLocalCharacteristic.CategoryId);

                if (category is null) continue;

                categories.Add(category);
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
            int hash = HashCode.Combine(obj.CategoryId, obj.Name, obj.Order);

            return hash;
        }
    }
}