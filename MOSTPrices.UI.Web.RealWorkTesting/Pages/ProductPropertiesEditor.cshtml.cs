using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.UI.Web.RealWorkTesting.Utils;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductProperties;
using Microsoft.AspNetCore.Authorization;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductToDisplayDataMappingUtils;
using System.Collections.Generic;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MOSTComputers.Services.ProductImageFileManagement.Services;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages;

[Authorize]
public class ProductPropertiesEditorModel : PageModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ProductPropertiesEditorModel(
        IProductService productService,
        IProductCharacteristicService productCharacteristicService,
        ISearchStringOriginService searchStringOriginService,
        IProductTableDataService productTableDataService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductDeserializeService productDeserializeService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        _productService = productService;
        _productCharacteristicService = productCharacteristicService;
        _searchStringOriginService = searchStringOriginService;
        _productTableDataService = productTableDataService;
        _productWorkStatusesService = productWorkStatusesService;
        _productDeserializeService = productDeserializeService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productImageFileManagementService = productImageFileManagementService;

        ProductDisplayData = _productTableDataService.GetProductById(ProductId);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly IProductService _productService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IProductTableDataService _productTableDataService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;

    [BindProperty(SupportsGet = true)]
    public int ProductId { get; set; }
    public ProductDisplayData? ProductDisplayData { get; set; }
    public List<ProductPropertyDisplayData>? ProductProperties { get; set; }
    public List<ProductPropertyByCharacteristicIdCreateRequest> ProductPropertyCreateRequests { get; set; }
    public IEnumerable<SelectListItem>? CharacteristicsForProductCategory { get; set; }

    public void OnGet()
    {
        if (ProductId <= 0) return;

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData == null) return;

        ProductDisplayData = productDisplayData;

        ProductProperties = productDisplayData.Properties;

        CharacteristicsForProductCategory = GetCharacteristicsForProductCategory((uint?)productDisplayData.CategoryId);
    }

    public IActionResult OnGetGetPartialViewXmlForProduct()
    {
        ProductDisplayData? productData = _productTableDataService.GetProductById(ProductId);

        if (productData == null) return BadRequest();

        Product product = MapToProduct(productData);

        OneOf<string, InvalidXmlResult> productXmlResult = _productDeserializeService.TrySerializeProductXml(product, true);

        return productXmlResult.Match<IStatusCodeActionResult>(
            xml =>
            {
                return Partial("ProductPopups/_ProductGeneratedXmlPopupPartial", new ProductGeneratedXmlPopupPartialModel()
                {
                    Product = product,
                    XmlData = xml,
                    NotificationBoxId = "topNotificationBox"
                });
            },
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    public IActionResult OnGetGetPartialViewImagesForProduct()
    {
        ProductDisplayData? productData = _productTableDataService.GetProductById(ProductId);

        if (productData == null) return BadRequest();

        return Partial("ProductPopups/_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(
            ProductImagePopupUsageEnum.DisplayData,
            productData,
            _productImageService,
            _productImageFileNameInfoService,
            _productImageFileManagementService,
            "ProductImages_popup_modal_content"));
    }

    public IActionResult OnGetGetRemainingCharacteristicsForProduct(int? characteristicToAddToSelectId = null)
    {
        if (characteristicToAddToSelectId is not null
            && characteristicToAddToSelectId < 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData is null) return BadRequest();

        IEnumerable<SelectListItem>? characteristicsForProductCategory = GetCharacteristicsForProductCategory((uint?)productDisplayData.CategoryId);

        IEnumerable<SelectListItem>? remainingCharacteristics
            = GetRemainingCharacteristics(productDisplayData, characteristicsForProductCategory, false);

        if (remainingCharacteristics is null) return new JsonResult(null);

        if (characteristicToAddToSelectId is not null)
        {
            SelectListItem? characteristicToAddSelectListItem = characteristicsForProductCategory?.FirstOrDefault(
                characteristic => characteristic.Value == characteristicToAddToSelectId.ToString());

            if (characteristicToAddSelectListItem is null) return BadRequest();

            remainingCharacteristics = remainingCharacteristics.Prepend(characteristicToAddSelectListItem);
        }

        return new JsonResult(remainingCharacteristics.Select(
            selectListItem => new { text = selectListItem.Text, value = selectListItem.Value }));
    }
    
    private IEnumerable<SelectListItem>? GetCharacteristicsForProductCategory(uint? categoryId)
    {
        if (categoryId == null) return null;

        IEnumerable<ProductCharacteristic> characteristicsForProductCategory
            = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId((int)categoryId.Value);

        return ProductSelectListItemUtils.GetCharacteristicSelectListItems(characteristicsForProductCategory);
    }

    public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        if (ProductDisplayData is null) return null;

        Product productFromDisplayData = MapToProduct(ProductDisplayData);

        return _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(productFromDisplayData);
    }

    public IEnumerable<SelectListItem>? GetRemainingCharacteristics(
        ProductDisplayData? productDisplayData,
        IEnumerable<SelectListItem>? characteristicsForProductCategory,
        bool addDefaultSelectACharacteristicText = true)
    {
        if (productDisplayData is null) return null;

        IEnumerable<string?>? productPropNames = productDisplayData.Properties?.Select(prop => prop.Characteristic);

        if (productPropNames is null)
        {
            return characteristicsForProductCategory?
                .Where(x => !string.IsNullOrEmpty(x.Text));
        }

        IEnumerable<SelectListItem>? remainingCharacteristics = characteristicsForProductCategory?
            .DistinctBy(x => x.Text)
            .ExceptBy(productPropNames,
                characteristic => characteristic.Text)
            .Where(x => !string.IsNullOrEmpty(x.Text));

        if (!addDefaultSelectACharacteristicText) return remainingCharacteristics;

        return remainingCharacteristics?.Prepend(new SelectListItem()
        {
            Text = "-- Select a characteristic --",
            Value = null,
            Selected = true,
            Disabled = true
        });
    }

    public IEnumerable<SelectListItem>? GetRemainingCharacteristics(bool addDefaultSelectACharacteristicText = true)
    {
        if (ProductDisplayData is null) return null;

        IEnumerable<string?>? productPropNames = ProductDisplayData.Properties?.Select(prop => prop.Characteristic);

        if (productPropNames is null)
        {
            return GetCharacteristicsForProductCategory((uint?)ProductDisplayData.CategoryId)?
                .Where(x => !string.IsNullOrEmpty(x.Text));
        }

        IEnumerable<SelectListItem>? remainingCharacteristics = GetCharacteristicsForProductCategory((uint?)ProductDisplayData.CategoryId)?
            .DistinctBy(x => x.Text)
            .ExceptBy(productPropNames,
                characteristic => characteristic.Text)
            .Where(x => !string.IsNullOrEmpty(x.Text));

        if (!addDefaultSelectACharacteristicText) return remainingCharacteristics;

        return remainingCharacteristics?.Prepend(new SelectListItem()
        {
            Text = "-- Select a characteristic --",
            Value = null,
            Selected = true,
            Disabled = true
        });
    }

    public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringOriginData(int productId)
    {
        if (productId <= 0) return null;

        Product? product = _productService.GetByIdWithProps(productId);

        if (product == null) return null;

        return _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(product);
    }

    public IActionResult OnPostAddNewItem()
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData == null) return BadRequest();

        ProductDisplayData = productDisplayData;

        IEnumerable<SelectListItem>? remainingCharacteristics = GetRemainingCharacteristics(false);

        SelectListItem? firstCharacteristic = remainingCharacteristics?.First();

        string? firstCharacteristicIdAsString = firstCharacteristic?.Value;

        int? characteristicId = null;

        if (firstCharacteristicIdAsString is not null)
        {
            bool success = int.TryParse(firstCharacteristicIdAsString, out int firstCharacteristicIdParsed);

            if (success)
            {
                characteristicId = firstCharacteristicIdParsed;
            }
        }

        ProductPropertyDisplayData productPropertyData = new()
        {
            ProductCharacteristicId = characteristicId,
            Characteristic = firstCharacteristic?.Text,
        };

        productDisplayData.Properties ??= new();

        productDisplayData.Properties.Add(productPropertyData);
        
        return base.Partial("ProductProperties/_ProductSinglePropertyDisplayPartial",
            new ProductSinglePropertyDisplayPartialModel(
                ProductId,
                productPropertyData,
                productDisplayData.Properties?.Count ?? 0,
                true,
                remainingCharacteristics,
                "topNotificationBox"));
    }

    public IActionResult OnPutUpdateProperty([FromBody] ProductPropertyEditorData data)
    {
        if (data is null
            || data.ProductCharacteristicId == 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData is null
            || productDisplayData.CategoryId is null) return BadRequest();

        ProductCharacteristic? propCharacteristic
            = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId((int)productDisplayData.CategoryId)
            .FirstOrDefault(characteristic => characteristic.Id == data.ProductCharacteristicId);

        if (propCharacteristic is null) return BadRequest();

        ProductPropertyDisplayData? propertyToUpdate = productDisplayData.Properties?
            .Find(prop => prop.ProductCharacteristicId == data.ProductCharacteristicId);

        if (data.IsNew && propertyToUpdate == null)
        {
            productDisplayData.Properties ??= new();

            productDisplayData.Properties.Add(new()
            {
                ProductCharacteristicId = data.ProductCharacteristicId,
                XmlPlacement = data.XmlPlacement,
                Value = data.Value,
                Characteristic = propCharacteristic.Name,
                DisplayOrder = null,
            });

            OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> setProductDataResultLocal
               = SetProductStatusToWorkInProgress(ProductId);

            return setProductDataResultLocal.Match(
                success => new OkResult(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(500),
                notFound => NotFound());
        }

        productDisplayData.Properties![productDisplayData.Properties.IndexOf(propertyToUpdate!)] = new()
        {
            ProductCharacteristicId = data.ProductCharacteristicId,
            XmlPlacement = data.XmlPlacement,
            Value = data.Value,
            DisplayOrder = propertyToUpdate?.DisplayOrder,
            Characteristic = propertyToUpdate?.Characteristic,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> setProductDataResult
            = SetProductStatusToWorkInProgress(ProductId);

        return setProductDataResult.Match(
            success => new OkResult(),
            validationResult => new OkResult(),
            unexpectedFailureResult => StatusCode(500),
            notFound => NotFound());
    }

    public IActionResult OnPutChangePropertyCharacteristicId(int currentPropertyId, int newPropertyId)
    {
        if (currentPropertyId <= 0
            || newPropertyId <= 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData is null) return BadRequest();

        CharacteristicsForProductCategory = GetCharacteristicsForProductCategory((uint?)productDisplayData.CategoryId);

        SelectListItem? characteristicItemWithNewId = CharacteristicsForProductCategory?.FirstOrDefault(x => x.Value == newPropertyId.ToString());

        if (characteristicItemWithNewId == null) return BadRequest();
        
        ProductPropertyDisplayData? propertyToChange = productDisplayData.Properties?
            .FirstOrDefault(property => property.ProductCharacteristicId == currentPropertyId);

        if (propertyToChange == null) return BadRequest();

        propertyToChange.ProductCharacteristicId = newPropertyId;
        propertyToChange.Characteristic = characteristicItemWithNewId.Text;

        OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> setProductDataResult = SetProductStatusToWorkInProgress(ProductId);

        return new OkResult();
    }

    public IActionResult OnPutUpdateAndInsertProperties([FromBody] List<ProductPropertyEditorData> data)
    {
        if (data is null) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData == null) return BadRequest();

        ProductDisplayData = productDisplayData;

        List<ProductPropertyDisplayData> properties = new();

        foreach (ProductPropertyEditorData productPropertyEditorData in data)
        {
            properties.Add(new()
            {
                ProductCharacteristicId = productPropertyEditorData.ProductCharacteristicId,
                XmlPlacement = productPropertyEditorData.XmlPlacement,
                Value = productPropertyEditorData.Value,
                DisplayOrder = null,
                Characteristic = null,
            });
        }

        productDisplayData.Properties = properties;

        OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> setProductDataResult = SetProductStatusToWorkInProgress(ProductId);

        return setProductDataResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500),
            notFound => NotFound());
    }

    public IActionResult OnDeleteDeleteProperty(uint productCharacteristicId)
    {
        if (ProductId <= 0
            || productCharacteristicId == 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(ProductId);

        if (productDisplayData is null) return BadRequest();

        ProductPropertyDisplayData? propertyData = productDisplayData.Properties?
            .FirstOrDefault(property => property.ProductCharacteristicId == productCharacteristicId);

        if (propertyData is null) return BadRequest();

        bool? success = productDisplayData.Properties?.Remove(propertyData);

        if (success is null || !(bool)success)
        {
            return NotFound();
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> setProductDataResult = SetProductStatusToWorkInProgress(ProductId);

        return new OkResult();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound> SetProductStatusToWorkInProgress(int productId)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null) return new NotFound();

        if (productDisplayData.ProductWorkStatusesId is null
            || productDisplayData.ProductWorkStatusesId <= 0)
        {
            ProductWorkStatusesCreateRequest productWorkStatusesCreateRequest = new()
            {
                ProductId = productId,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = productDisplayData.ReadyForImageInsert ?? false,
            };
            
            OneOf<int, ValidationResult, UnexpectedFailureResult> productWorkStatusesInsertResult
                = _productWorkStatusesService.InsertIfItDoesntExist(productWorkStatusesCreateRequest);

            return productWorkStatusesInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound>>(
                id =>
                {
                    productDisplayData.ProductWorkStatusesId = id;

                    productDisplayData.ProductNewStatus = ProductNewStatusEnum.WorkInProgress;
                    productDisplayData.ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress;

                    return new Success();
                },
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        ProductWorkStatusesUpdateByIdRequest productWorkStatusesUpdateRequest = new()
        {
            Id = productDisplayData.ProductWorkStatusesId.Value,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = productDisplayData.ReadyForImageInsert ?? false,
        };

        OneOf<bool, ValidationResult> productWorkStatusesUpdateResult = _productWorkStatusesService.UpdateById(productWorkStatusesUpdateRequest);

        return productWorkStatusesUpdateResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, NotFound>>(
            isSuccessful =>
            {
                if (!isSuccessful) return new UnexpectedFailureResult();

                productDisplayData.ProductNewStatus = ProductNewStatusEnum.WorkInProgress;
                productDisplayData.ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress;

                return new Success();
            },
            validationResult => validationResult);
    }
}