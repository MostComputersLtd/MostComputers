using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Pages.Shared;
using MOSTComputers.UI.Web.Pages.Shared.ProductProperties;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static MOSTComputers.UI.Web.Mapping.XmlPlacementEnumMapping;

namespace MOSTComputers.UI.Web.Pages;

public class ProductPropertiesEditorModel : PageModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ProductPropertiesEditorModel(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductXmlToCreateRequestMappingService mapperService,
        IProductDeserializeService productDeserializeService,
        ISearchStringOriginService searchStringOriginService)
    {
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productCharacteristicService = productCharacteristicService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _mapperService = mapperService;
        _productDeserializeService = productDeserializeService;
        _searchStringOriginService = searchStringOriginService;
    }

    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductXmlToCreateRequestMappingService _mapperService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly ISearchStringOriginService _searchStringOriginService;

    [BindProperty(SupportsGet = true)]
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public List<ProductProperty> ProductProperties { get; set; }
    public List<ProductPropertyByCharacteristicIdCreateRequest> ProductPropertyCreateRequests { get; set; }
    public IEnumerable<SelectListItem> CharacteristicsForProductCategory { get; set; }

    public void OnGet()
    {
        if (ProductId <= 0) return;

        Product? product = _productService.GetByIdWithProps((uint)ProductId);

        if (product == null) return;

        Product = product;

        ProductProperties = product.Properties;

        InitializeCharacteristicsForProductCategory((uint?)product.CategoryID);
    }

    private void InitializeCharacteristicsForProductCategory(uint? categoryId)
    {
        if (categoryId == null) return;

        IEnumerable<ProductCharacteristic> characteristicsForProductCategory = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId((int)categoryId.Value);

        CharacteristicsForProductCategory = characteristicsForProductCategory
            .Select(productCharacteristic =>
            new SelectListItem()
            {
                Text = productCharacteristic.Name,
                Value = productCharacteristic.Id.ToString(),
            });
    }

    public IActionResult OnGetPartialViewXmlForProduct()
    {
        Product? product = _productService.GetByIdWithProps((uint)ProductId);

        if (product == null) return BadRequest();

        product.Images = _productImageService.GetAllInProduct((uint)ProductId)
            .ToList();

        Product = product;

        string? productXml = GetProductXML(Product);

        if (productXml is null) return BadRequest();

        return Partial("_ProductGeneratedXmlPopupPartial", new ProductGeneratedXmlPopupPartialModel() { XmlData = productXml, Product = Product });
    }

    public IActionResult OnGetPartialViewImagesForProduct()
    {
        Product? product = _productService.GetByIdWithImages((uint)ProductId);

        if (product == null) return BadRequest();

        product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct((uint)product.Id)
            .ToList();

        Product = product;

        return Partial("_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupModel(Product));
    }

    public IActionResult OnGetCurrentImageFileResultSingle(uint imageIndex)
    {
        Product? product = _productService.GetByIdWithImages((uint)ProductId);

        if (product == null) return BadRequest();

        Product = product;

        if (Product is null
            || Product.Images is null
            || Product.Images.Count <= imageIndex) return BadRequest();

        ProductImage image = Product.Images[(int)imageIndex];

        if (image.ImageData is null
            || image.ImageFileExtension is null) return StatusCode(500);

        int slashIndex = image.ImageFileExtension.IndexOf('/');

        string fileExtension = image.ImageFileExtension[(slashIndex + 1)..];

        return base.File(image.ImageData, image.ImageFileExtension, $"{Product.Id}_{imageIndex}.{fileExtension}");
    }

    private string? GetProductXML(Product product)
    {
        XmlObjectData xmlDataFromProduct = _mapperService.GetXmlDataFromProducts(new List<Product>() { product });

        OneOf<string?, InvalidXmlResult> serializationResult = _productDeserializeService.TrySerializeProductsXml(xmlDataFromProduct, true);

        return serializationResult.Match(
            data => data,
            invalidXmlResult => null);
    }

    public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        return _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(Product);
    }

    public IActionResult OnGetGetSearchStringPartialView()
    {
        Product? product = _productService.GetByIdWithFirstImage((uint)ProductId);

        if (product == null) return BadRequest();

        Product = product;

        if (product.SearchString is null) return new OkResult();

        IEnumerable<ProductCharacteristic>? characteristicAndSearchStringAbbreviations = null;

        if (product.CategoryID is not null)
        {
            characteristicAndSearchStringAbbreviations = _productCharacteristicService.GetAllByCategoryId((int)product.CategoryID);
        }

        return base.Partial("_ProductSearchStringDisplayPopupPartial", new ProductSearchStringDisplayPopupPartialModel()
        {
            Product = product,
            CharacteristicsAndSearchStringAbbreviationsForProduct = characteristicAndSearchStringAbbreviations ?? Array.Empty<ProductCharacteristic>(),
            SearchStringOriginService = _searchStringOriginService,
        });
    }

    public IEnumerable<SelectListItem> GetRemainingCharacteristics()
    {
        IEnumerable<string?> productPropNames = Product.Properties.Select(prop => prop.Characteristic);

        return CharacteristicsForProductCategory
            .DistinctBy(x => x.Text)
            .ExceptBy(productPropNames,
                characteristic => characteristic.Text)
            .Where(x => !string.IsNullOrEmpty(x.Text))
            .Prepend(new SelectListItem()
            {
                Text = "-- Select a characteristic --",
                Value = null,
                Selected = true,
                Disabled = true
            });
    }

    public IActionResult OnPostAddNewItem()
    {
        Product? product = _productService.GetByIdWithProps((uint)ProductId);

        if (product == null) return BadRequest();

        Product = product;

        InitializeCharacteristicsForProductCategory((uint?)product.CategoryID);

        return Partial("ProductProperties/_ProductPropertyWithoutCharacteristicDisplayForPropertyEditorPartial",
            new ProductPropertyWithoutCharacteristicDisplayForPropertyEditorPartialModel()
            {
                ProductProperty = new ProductProperty() { ProductCharacteristicId = null, ProductId = ProductId },
                ProductCharacteristicsForSelect = GetRemainingCharacteristics(),
                PropertyIndex = (uint)product.Properties.Count,
            });
    }

    public IActionResult OnPutUpdateProperty([FromBody] ProductPropertyEditorData data)
    {
        if (data is null
            || data.ProductCharacteristicId == 0) return BadRequest();

        if (data.IsNew)
        {
            ProductPropertyByCharacteristicIdCreateRequest productPropertyCreateRequest = new()
            {
                ProductCharacteristicId = data.ProductCharacteristicId,
                ProductId = ProductId,
                XmlPlacement = data.XmlPlacement,
                Value = data.Value,
                DisplayOrder = null,
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> createResult = _productPropertyService.InsertWithCharacteristicId(productPropertyCreateRequest);

            return createResult.Match<IActionResult>(
                _ => new OkResult(),
                validationResult => BadRequest(validationResult),
                _ => StatusCode(500));
        }

        ProductPropertyUpdateRequest productPropertyUpdateRequest = new()
        {
            ProductCharacteristicId = data.ProductCharacteristicId,
            ProductId = ProductId,
            XmlPlacement = data.XmlPlacement,
            Value = data.Value,
            DisplayOrder = null,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productPropertyService.Update(productPropertyUpdateRequest);

        return updateResult.Match<IActionResult>(
            _ => new OkResult(),
            validationResult => BadRequest(validationResult),
            _ => StatusCode(500));
    }

    public IActionResult OnPutUpdateAndInsertProperties([FromBody] List<ProductPropertyEditorData> data)
    {
        if (data is null) return BadRequest();

        Product? product = _productService.GetByIdWithProps((uint)ProductId);

        if (product == null) return BadRequest();

        Product = product;

        List<ProductPropertyByCharacteristicIdCreateRequest> propertyCharacteristicIdCreateRequests = new();
        List<ProductPropertyUpdateRequest> propertyUpdateRequests = new();

        foreach (ProductPropertyEditorData propertyUIData in data)
        {
            if (propertyUIData.IsNew)
            {
                ProductPropertyByCharacteristicIdCreateRequest productPropertyByCharacteristicIdCreateRequest = new()
                {
                    ProductCharacteristicId = propertyUIData.ProductCharacteristicId,
                    Value = propertyUIData.Value,
                    ProductId = ProductId,
                    XmlPlacement = propertyUIData.XmlPlacement
                };

                propertyCharacteristicIdCreateRequests.Add(productPropertyByCharacteristicIdCreateRequest);

                continue;
            }

            ProductProperty? propertyInOriginalProduct = product.Properties.FirstOrDefault(x => x.ProductCharacteristicId == propertyUIData.ProductCharacteristicId);

            if (propertyInOriginalProduct is null) return StatusCode(500);

            if (propertyUIData.XmlPlacement == propertyInOriginalProduct.XmlPlacement
                && propertyUIData.Value == propertyInOriginalProduct.Value) continue;

            ProductPropertyUpdateRequest productPropertyUpdateRequest = new()
            {
                ProductCharacteristicId = propertyUIData.ProductCharacteristicId,
                ProductId = ProductId,
                XmlPlacement = propertyUIData.XmlPlacement ?? propertyInOriginalProduct.XmlPlacement,
                Value = propertyUIData.Value ?? propertyInOriginalProduct.Value,
                DisplayOrder = propertyInOriginalProduct.DisplayOrder,
            };

            propertyUpdateRequests.Add(productPropertyUpdateRequest);
        }

        ProductUpdateRequest productUpdateRequest = new()
        {
            Id = product.Id,
            Name = product.Name,
            AdditionalWarrantyPrice = product.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = product.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = product.StandardWarrantyPrice,
            StandardWarrantyTermMonths = product.StandardWarrantyTermMonths,
            DisplayOrder = product.DisplayOrder,
            Status = product.Status,
            PlShow = product.PlShow,
            DisplayPrice = product.Price,
            Currency = product.Currency,
            RowGuid = product.RowGuid,
            Promotionid = product.Promotionid,
            PromRid = product.PromRid,
            PromotionPictureId = product.PromotionPictureId,
            PromotionExpireDate = product.PromotionExpireDate,
            AlertPictureId = product.AlertPictureId,
            AlertExpireDate = product.AlertExpireDate,
            PriceListDescription = product.PriceListDescription,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            SearchString = product.SearchString,
            Properties = propertyUpdateRequests.Select(Map)
                .ToList(),
            Images = null,
            ImageFileNames = null,
            CategoryID = product.CategoryID,
            ManifacturerId = product.ManifacturerId,
            SubCategoryId = product.SubCategoryId,
        };

        if (propertyUpdateRequests.Count > 0)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

            ValidationResult? validationResult = null;

            IActionResult updateActionResult = productUpdateResult.Match<IActionResult>(
                _ => new OkResult(),
                valResult =>
                {
                    validationResult = valResult;

                    return BadRequest(validationResult);
                },
                _ => StatusCode(500));

            if (updateActionResult is not OkResult)
            {
                return updateActionResult;
            }
        }

        foreach (ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest in propertyCharacteristicIdCreateRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> createPropResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);
        }

        return new OkResult();
    }

    public IActionResult OnDeleteDeleteProperty(uint productCharacteristicId)
    {
        if (ProductId <= 0
            || productCharacteristicId == 0) return BadRequest();

        bool deleteResult = _productPropertyService.Delete((uint)ProductId, productCharacteristicId);

        if (!deleteResult)
        {
            return NotFound();
        }

        return new OkResult();
    }

    private static CurrentProductPropertyUpdateRequest Map(ProductPropertyUpdateRequest updateRequest)
    {
        return new()
        {
            ProductCharacteristicId = updateRequest.ProductCharacteristicId,
            DisplayOrder = updateRequest.DisplayOrder,
            Value = updateRequest.Value,
            XmlPlacement = updateRequest.XmlPlacement,
        };
    }
}