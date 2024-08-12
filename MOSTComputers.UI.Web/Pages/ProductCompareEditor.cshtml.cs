using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Pages.Shared.ProductCompareEditor;
using MOSTComputers.UI.Web.Pages.Shared.ProductCompareEditor.ProductProperties;
using MOSTComputers.UI.Web.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;
using System.Web;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public class ProductCompareEditorModel : PageModel
{
    public ProductCompareEditorModel(
        IProductService productService,
        IProductDeserializeService productDeserializeService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        ITransactionExecuteService transactionExecuteService,
        ISearchStringOriginService searchStringOriginService,
        IProductXmlToCreateRequestMappingService productXmlToCreateRequestMappingService,
        IProductToXmlProductMappingService productToXmlProductMappingService,
        IProductXmlToProductMappingService productXmlToProductMappingService)
    {
        _productService = productService;
        _productDeserializeService = productDeserializeService;
        _productCharacteristicService = productCharacteristicService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _transactionExecuteService = transactionExecuteService;
        _searchStringOriginService = searchStringOriginService;
        _productXmlToCreateRequestMappingService = productXmlToCreateRequestMappingService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
        _productXmlToProductMappingService = productXmlToProductMappingService;
    }

    private readonly IProductService _productService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IProductXmlToCreateRequestMappingService _productXmlToCreateRequestMappingService;
    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;
    private readonly IProductXmlToProductMappingService _productXmlToProductMappingService;

    [BindProperty(SupportsGet = true)]
    public int? FirstProductId { get; set; }

    private static string? _firstProductXml = null;

    private static XmlObjectData? _firstProductXmlObjectData = null;

    private static Product? _firstProduct = null;
    public Product? FirstProduct { get => _firstProduct; set => _firstProduct = value; }

    private static List<Tuple<string, List<SearchStringPartOriginData>?>>? _firstProductSearchStringPartsAndDataAboutTheirOrigin = null;
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? FirstProductSearchStringPartsAndDataAboutTheirOrigin
    {
        get => _firstProductSearchStringPartsAndDataAboutTheirOrigin;
        set => _firstProductSearchStringPartsAndDataAboutTheirOrigin = value;
    }

    private static List<ProductCharacteristic>? _characteristicsRelatedToFirstProduct = null;
    public List<ProductCharacteristic>? CharacteristicsRelatedToFirstProduct
    {
        get => _characteristicsRelatedToFirstProduct;
        set => _characteristicsRelatedToFirstProduct = value;
    }

    public static List<(ProductImage productImage, int displayOrder)> OrdersForImagesInFirstProduct { get; } = new();

    private static string? _secondProductXml = null;

    private static XmlObjectData? _secondProductXmlObjectData = null;

    private static Product? _secondProduct = null;
    public Product? SecondProduct { get => _secondProduct; set => _secondProduct = value; }

    private static List<Tuple<string, List<SearchStringPartOriginData>?>>? _secondProductSearchStringPartsAndDataAboutTheirOrigin = null;
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? SecondProductSearchStringPartsAndDataAboutTheirOrigin
    {
        get => _secondProductSearchStringPartsAndDataAboutTheirOrigin;
        set => _secondProductSearchStringPartsAndDataAboutTheirOrigin = value;
    }

    private static List<ProductCharacteristic>? _characteristicsRelatedToSecondProduct = null;
    public List<ProductCharacteristic>? CharacteristicsRelatedToSecondProduct
    {
        get => _characteristicsRelatedToSecondProduct;
        set => _characteristicsRelatedToSecondProduct = value;
    }

    public static List<(ProductImage productImage, ProductImageFileNameInfo productImageFileNameInfo)> RelatedImagesAndFileInfosInSecondProduct { get; } = new();

    public IActionResult OnGetGetProductPartialViewFirst()
    {
        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnGetGetRefreshedProductPartialViewFirst()
    {
        if (_firstProduct is null) return BadRequest();

        GetRefreshedFirstProductAndPopulateItsData(_firstProduct.Id);

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnGetGetProductPartialViewSecond()
    {
        if (_secondProduct is not null)
        {
            OrderImagesInProductByDisplayOrder(_secondProduct, RelatedImagesAndFileInfosInSecondProduct);
        }

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));
    }

    public IActionResult OnGetGetXmlFromProductFirst()
    {
        if (_firstProduct is null) return new OkObjectResult(null);

        if (_firstProductXmlObjectData is not null)
        {
            XmlProduct? oldXmlProduct = _firstProductXmlObjectData.Products.FirstOrDefault();

            string? vendorUrl = oldXmlProduct?.VendorUrl;

            _firstProductXmlObjectData.Products.Clear();

            XmlProduct xmlFirstProduct = _productToXmlProductMappingService.MapToXmlProduct(_firstProduct);

            _firstProductXmlObjectData.Products.Add(xmlFirstProduct);

            xmlFirstProduct.VendorUrl ??= vendorUrl;

            _firstProductXml = _productDeserializeService.SerializeProductsXml(_firstProductXmlObjectData, true, true);

            return new OkObjectResult(_firstProductXml);
        }

        _firstProductXml = _productDeserializeService.SerializeProductXml(_firstProduct, true, true);

        return new OkObjectResult(_firstProductXml);
    }

    public IActionResult OnGetGetXmlFromProductSecond()
    {
        if (_secondProduct is null) return new OkObjectResult(null);

        if (_secondProductXmlObjectData is not null)
        {
            XmlProduct? oldXmlProduct = _secondProductXmlObjectData.Products.FirstOrDefault();

            string? vendorUrl = oldXmlProduct?.VendorUrl;

            _secondProductXmlObjectData.Products.Clear();

            XmlProduct xmlSecondProduct = _productToXmlProductMappingService.MapToXmlProduct(_secondProduct);

            _secondProductXmlObjectData.Products.Add(xmlSecondProduct);

            xmlSecondProduct.VendorUrl ??= vendorUrl;

            _secondProductXml = _productDeserializeService.SerializeProductsXml(_secondProductXmlObjectData, true, true);

            return new OkObjectResult(_secondProductXml);
        }

        _secondProductXml = _productDeserializeService.SerializeProductXml(_secondProduct, true, true);

        return new OkObjectResult(_secondProductXml);
    }

    public IActionResult OnPostGetProductXmlByIdFirst(int productId)
    {
        if (productId < 0) return BadRequest();

        if (_firstProduct is not null
            && _firstProduct.Id == productId)
        {
            if (_firstProductXml is not null) return new OkObjectResult(_firstProductXml);

            return OnGetGetXmlFromProductFirst();
        }

        GetRefreshedFirstProductAndPopulateItsData(productId);

        return OnGetGetXmlFromProductFirst();
    }
    
    public IActionResult OnPostGetProductDataByIdFirst(int productId)
    {
        if (productId < 0) return BadRequest();

        if (_firstProduct is not null
            && productId != _firstProduct.Id)
        {
            _firstProduct = null;
        }
        
        GetFirstProductAndPopulateItsData(productId);

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    private Product? GetFirstProductAndPopulateItsData(int productId)
    {
        return GetFirstProductDataAndPopulateIt(productId, _firstProduct);
    }

    private Product? GetRefreshedFirstProductAndPopulateItsData(int productId)
    {
        return GetFirstProductDataAndPopulateIt(productId);
    }

    private Product? GetFirstProductDataAndPopulateIt(int productId, Product? product = null)
    {
        if (productId <= 0) return null;

        Product? firstProduct = null;

        if (product is not null
            && productId == product.Id)
        {
            firstProduct = product;
        }

        if (firstProduct is null)
        {
            firstProduct = _productService.GetByIdWithImages(productId);

            if (firstProduct is null)
            {
                _firstProduct = null;
                _firstProductSearchStringPartsAndDataAboutTheirOrigin = null;
                _characteristicsRelatedToFirstProduct = null;

                return null;
            }
        }

        if ((firstProduct.Images is null || firstProduct.Images.Count <= 0)
            && product is not null
            && product.Images?.Count > 0)
        {
            firstProduct.Images = product.Images;
        }

        if (firstProduct is null)
        {
            _firstProduct = null;
            _firstProductSearchStringPartsAndDataAboutTheirOrigin = null;
            _characteristicsRelatedToFirstProduct = null;

            return null;
        }

        if (firstProduct.Properties is null
            || firstProduct.Properties.Count <= 0)
        {
            firstProduct.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        if (firstProduct.ImageFileNames is null
            || firstProduct.ImageFileNames.Count <= 0)
        {
            firstProduct.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        List<Tuple<string, List<SearchStringPartOriginData>?>>? productSearchStringOriginData
            = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(firstProduct!);

        List<int> categoryIds = new() { -1 };

        if (firstProduct!.CategoryID is not null)
        {
            categoryIds.Add(firstProduct.CategoryID.Value);
        }

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsRelatedToProduct =
            _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds);

        OrdersForImagesInFirstProduct.Clear();

        firstProduct.Images = firstProduct.Images?.OrderBy(
            image =>
            {
                ProductImageFileNameInfo? relatedFileName = firstProduct.ImageFileNames?.Find(
                    x => x.FileName == $"{image.Id}.{GetFileExtensionFromFileType(image.ImageContentType ?? "*")}");

                if (relatedFileName is not null) return relatedFileName.DisplayOrder;

                int imageIndex = firstProduct.Images!.IndexOf(image);

                OrdersForImagesInFirstProduct.Add((image, imageIndex + 1));

                return imageIndex;
            })
            .ToList();

        _firstProduct = CloneProduct(firstProduct);
        _firstProductSearchStringPartsAndDataAboutTheirOrigin = productSearchStringOriginData;
        _characteristicsRelatedToFirstProduct = characteristicsRelatedToProduct
            .SelectMany(grouping => grouping.AsEnumerable())
            .ToList();

        return firstProduct;
    }

    public async Task<IActionResult> OnPostGetProductDataFromXmlSecondAsync([FromBody] string xmlData)
    {
        if (string.IsNullOrWhiteSpace(xmlData)) return BadRequest();

        OneOf<XmlObjectData?, InvalidXmlResult> productDeserializeResult
            = _productDeserializeService.TryDeserializeProductsXml(xmlData);

        return await productDeserializeResult.Match<Task<IActionResult>>(
            async xmlObjectData =>
            {
                if (xmlObjectData is null) return BadRequest();

                if (xmlObjectData.Products.Count > 1) return BadRequest();

                XmlProduct? xmlProduct = xmlObjectData.Products.ElementAtOrDefault(0);

                if (xmlProduct is null) return Partial("ProductCompareEditor/_ProductFullEditorPartial",
                    GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));

                OneOf<Product, ValidationResult> productFromXmlProductMappingResult
                    = await _productXmlToProductMappingService.GetProductFromXmlDataAsync(xmlProduct, xmlData);

                Product? product = null;
                ValidationResult? failedMappingValidationResult = productFromXmlProductMappingResult.Match<ValidationResult?>(
                    prod =>
                    {
                        product = prod;

                        return null;
                    },
                    validationResult => validationResult);

                if (failedMappingValidationResult is not null)
                {
                    return BadRequest(failedMappingValidationResult);
                }

                List<Tuple<string, List<SearchStringPartOriginData>?>>? productSearchStringOriginData
                    = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(product!);

                List<int> categoryIds = new() { -1 };

                if (product!.CategoryID is not null)
                {
                    categoryIds.Add(product.CategoryID.Value);
                }

                IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsRelatedToProduct =
                    _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds);

                RelatedImagesAndFileInfosInSecondProduct.Clear();

                if (_secondProduct?.Images is not null
                    && _secondProduct.Images.Count > 0
                    && product.Images is not null)
                {
                    foreach (ProductImage image in product.Images)
                    {
                        if (image.ImageData is not null) continue;

                        ProductImage? matchingImageInOldSecondProduct = _secondProduct.Images.Find(imageInOld =>
                            imageInOld.Id == image.Id);

                        if (matchingImageInOldSecondProduct is null) continue;

                        image.ImageData = matchingImageInOldSecondProduct?.ImageData;
                    }
                }

                if (product.Images is not null)
                {
                    for (int i = 0; i < product.Images.Count; i++)
                    {
                        ProductImage image = product.Images[i];
                        ProductImageFileNameInfo? imageFileNameInfo = product.ImageFileNames?.ElementAtOrDefault(i);

                        if (imageFileNameInfo is not null)
                        {
                            RelatedImagesAndFileInfosInSecondProduct.Add((image, imageFileNameInfo));
                        }
                    }
                }

                _secondProductXmlObjectData = xmlObjectData;
                _secondProduct = CloneProduct(product);
                _secondProductXml = xmlData;
                _secondProductSearchStringPartsAndDataAboutTheirOrigin = productSearchStringOriginData;
                _characteristicsRelatedToSecondProduct = characteristicsRelatedToProduct
                    .SelectMany(grouping => grouping.AsEnumerable())
                    .ToList();

                return Partial("ProductCompareEditor/_ProductFullEditorPartial",
                    GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));
            },
            invalidXmlResult => Task.FromResult<IActionResult>(BadRequest(invalidXmlResult)));
    }

    public IActionResult OnPostCopyOtherProductIntoProductFirst()
    {
        if (_secondProduct is null) return BadRequest();

        int? firstProductOriginalId = _firstProduct?.Id;

        _firstProduct = CloneProduct(_secondProduct);

        if (firstProductOriginalId is not null
            && _firstProduct is not null)
        {
            _firstProduct.Id = firstProductOriginalId.Value;
        }

        _firstProductSearchStringPartsAndDataAboutTheirOrigin = _secondProductSearchStringPartsAndDataAboutTheirOrigin;

        _characteristicsRelatedToFirstProduct = _characteristicsRelatedToSecondProduct;

        _firstProductXmlObjectData = _secondProductXmlObjectData;

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnPostCopyOtherProductIntoProductSecond()
    {
        if (_firstProduct is null) return BadRequest();

        _secondProduct = CloneProduct(_firstProduct);

        _secondProductSearchStringPartsAndDataAboutTheirOrigin = _firstProductSearchStringPartsAndDataAboutTheirOrigin;

        _characteristicsRelatedToSecondProduct = _characteristicsRelatedToFirstProduct;

        _secondProductXmlObjectData = _firstProductXmlObjectData;

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));
    }

    public IActionResult OnPutSaveFirstProduct()
    {
        if (_firstProduct is null) return BadRequest();

        try
        {
            int firstProductId = _firstProduct.Id;

            IStatusCodeActionResult result = _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                SaveFirstProductInternal,
                actionResult => actionResult.StatusCode == 200);

            if (result.StatusCode != 200)
            {
                return result;
            }

            _firstProduct = null;

            GetFirstProductAndPopulateItsData(firstProductId);

            return OnGetGetProductPartialViewFirst();
        }
        catch(TransactionException)
        {
            return StatusCode(500);
        }
    }

    private IStatusCodeActionResult SaveFirstProductInternal()
    {
        if (_firstProduct is null) return BadRequest();

        int productId = _firstProduct.Id;

        Product? oldFirstProduct = _productService.GetByIdWithImages(productId);

        if (oldFirstProduct is null) return BadRequest();

        if (oldFirstProduct.Properties is null
            || oldFirstProduct.Properties.Count <= 0)
        {
            oldFirstProduct.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        if (oldFirstProduct.ImageFileNames is null
            || oldFirstProduct.ImageFileNames.Count <= 0)
        {
            oldFirstProduct.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        ProductUpdateRequest productUpdateRequest = new()
        {
            Id = _firstProduct.Id,
            Name = _firstProduct.Name,
            AdditionalWarrantyPrice = _firstProduct.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = _firstProduct.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = _firstProduct.StandardWarrantyPrice,
            StandardWarrantyTermMonths = _firstProduct.StandardWarrantyTermMonths,
            DisplayOrder = _firstProduct.DisplayOrder,
            Status = _firstProduct.Status,
            PlShow = _firstProduct.PlShow,
            DisplayPrice = _firstProduct.Price,
            Currency = _firstProduct.Currency,
            RowGuid = _firstProduct.RowGuid,
            Promotionid = _firstProduct.Promotionid,
            PromRid = _firstProduct.PromRid,
            PromotionPictureId = _firstProduct.PromotionPictureId,
            PromotionExpireDate = _firstProduct.PromotionExpireDate,
            AlertPictureId = _firstProduct.AlertPictureId,
            AlertExpireDate = _firstProduct.AlertExpireDate,
            PriceListDescription = _firstProduct.PriceListDescription,
            PartNumber1 = _firstProduct.PartNumber1,
            PartNumber2 = _firstProduct.PartNumber2,
            SearchString = _firstProduct.SearchString,
            Properties = new(),

            Images = new(),
            ImageFileNames = new(),
            CategoryID = _firstProduct.CategoryID,
            ManifacturerId = _firstProduct.ManifacturerId,
            SubCategoryId = _firstProduct.SubCategoryId,
        };

        IStatusCodeActionResult propertiesUpdateActionResult = UpdatePropertiesForFirstProduct(oldFirstProduct, productId, productUpdateRequest);

        if (propertiesUpdateActionResult.StatusCode != 200)
        {
            return propertiesUpdateActionResult;
        }

        IStatusCodeActionResult imagesUpdateActionResult = UpdateImagesInFirstProduct(oldFirstProduct, productId, productUpdateRequest);

        if (imagesUpdateActionResult.StatusCode != 200)
        {
            return imagesUpdateActionResult;
        }

        IStatusCodeActionResult imageFileNamesUpdateActionResult = UpdateImageFileNamesInFirstProduct(oldFirstProduct, productId);

        if (imageFileNamesUpdateActionResult.StatusCode != 200)
        {
            return imageFileNamesUpdateActionResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        IStatusCodeActionResult actionResultFromUpdate = productUpdateResult.Match<IStatusCodeActionResult>(
            success => new OkResult(),
            validationResult => BadRequest(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (actionResultFromUpdate.StatusCode != 200)
        {
            return actionResultFromUpdate;
        }

        return new OkResult();
    }

    private IStatusCodeActionResult UpdatePropertiesForFirstProduct(
        Product oldFirstProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddPropsTo)
    {
        if (_firstProduct is null) return BadRequest();

        foreach (ProductProperty newProductProp in _firstProduct.Properties)
        {
            ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
                .Find(prop => prop.ProductCharacteristicId == newProductProp.ProductCharacteristicId);

            if (oldProductPropForSameCharacteristic is null)
            {
                ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
                {
                    ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                    ProductId = productId,
                    DisplayOrder = newProductProp.DisplayOrder,
                    Value = newProductProp.Value,
                    XmlPlacement = newProductProp.XmlPlacement,
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult
                    = _productPropertyService.InsertWithCharacteristicId(propCreateRequest);

                IStatusCodeActionResult actionResultFromPropertyInsert = propertyInsertResult.Match<IStatusCodeActionResult>(
                    success => new OkResult(),
                    validationResult => BadRequest(validationResult),
                    unexpectedFailureResult => StatusCode(500));

                if (actionResultFromPropertyInsert.StatusCode != 200)
                {
                    return actionResultFromPropertyInsert;
                }

                continue;
            }

            if (oldProductPropForSameCharacteristic.Value == newProductProp.Value
                && oldProductPropForSameCharacteristic.XmlPlacement == newProductProp.XmlPlacement)
            {
                oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);

                continue;
            }

            CurrentProductPropertyUpdateRequest propUpdateRequest = new()
            {
                ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                DisplayOrder = newProductProp.DisplayOrder,
                Value = newProductProp.Value,
                XmlPlacement = newProductProp.XmlPlacement,
            };

            productUpdateRequestToAddPropsTo.Properties.Add(propUpdateRequest);

            oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);
        }

        foreach (ProductProperty oldPropertyToDelete in oldFirstProduct.Properties)
        {
            if (oldPropertyToDelete.ProductCharacteristicId is null) return StatusCode(500);

            bool imageFileNameDeleteSuccess = _productPropertyService.Delete(
                productId, oldPropertyToDelete.ProductCharacteristicId.Value);

            if (!imageFileNameDeleteSuccess) return StatusCode(500);
        }

        return new OkResult();
    }

    private IStatusCodeActionResult UpdateImagesInFirstProduct(
        Product oldFirstProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddImagesTo)
    {
        if (_firstProduct is null) return BadRequest();

        if (_firstProduct.Images is not null)
        {
            foreach (ProductImage image in _firstProduct.Images)
            {
                ProductImage? imageInOldProduct = oldFirstProduct.Images?.Find(
                    img => img.Id == image.Id);

                if (imageInOldProduct is null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = productId,
                        ImageData = image.ImageData,
                        ImageContentType = image.ImageContentType,
                        HtmlData = image.HtmlData,
                    };

                    OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
                        = _productImageService.InsertInAllImages(productImageCreateRequest);

                    int imageId = -1;

                    IStatusCodeActionResult actionResultFromImageInsert = imageInsertResult.Match<IStatusCodeActionResult>(
                        id =>
                        {
                            imageId = id;

                            return new OkResult();
                        },
                        validationResult => BadRequest(validationResult),
                        unexpectedFailureResult => StatusCode(500));

                    if (actionResultFromImageInsert.StatusCode != 200)
                    {
                        return actionResultFromImageInsert;
                    }

                    int imageInOrderedListIndex = OrdersForImagesInFirstProduct.FindIndex(
                        x => x.productImage == image);

                    if (imageInOrderedListIndex <= -1) return StatusCode(500);

                    (ProductImage productImage, int displayOrder) = OrdersForImagesInFirstProduct[imageInOrderedListIndex];

                    productImage.Id = imageId;

                    OrdersForImagesInFirstProduct[imageInOrderedListIndex] = new(productImage, displayOrder);

                    continue;
                }

                if (CompareByteArrays(image.ImageData, imageInOldProduct.ImageData)
                    && image.ImageContentType == imageInOldProduct.ImageContentType
                    && image.HtmlData == imageInOldProduct.HtmlData)
                {
                    oldFirstProduct.Images?.Remove(imageInOldProduct);

                    continue;
                }

                CurrentProductImageUpdateRequest productImageUpdateRequest = new()
                {
                    ImageData = image.ImageData,
                    ImageContentType = image.ImageContentType,
                    HtmlData = image.HtmlData,
                };

                productUpdateRequestToAddImagesTo.Images ??= new();

                productUpdateRequestToAddImagesTo.Images.Add(productImageUpdateRequest);

                oldFirstProduct.Images?.Remove(imageInOldProduct);
            }
        }

        if (oldFirstProduct.Images is not null
            && oldFirstProduct.Images.Count > 0)
        {
            foreach (ProductImage oldImageToBeRemoved in oldFirstProduct.Images)
            {
                bool imageDeleteResult = _productImageService.DeleteInAllImagesById(oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return StatusCode(500);
            }
        }

        return new OkResult();
    }

    private IStatusCodeActionResult UpdateImageFileNamesInFirstProduct(
        Product oldFirstProduct,
        int productId)
    {
        if (_firstProduct is null) return BadRequest();

        List<ServiceProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests = new();

        if (oldFirstProduct.ImageFileNames is not null
            && oldFirstProduct.ImageFileNames.Count > 0)
        {
            if (_firstProduct.ImageFileNames is null
                || _firstProduct.ImageFileNames.Count <= 0)
            {
                foreach (ProductImageFileNameInfo oldProductImageFileName in oldFirstProduct.ImageFileNames)
                {
                    if (oldProductImageFileName.DisplayOrder is null) return StatusCode(500);

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return StatusCode(500);

                    for (int k = 0; k < oldFirstProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldFirstProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldFirstProduct.ImageFileNames.Count; i++)
                {
                    ProductImageFileNameInfo oldProductImageFileName = oldFirstProduct.ImageFileNames[i];

                    if (oldProductImageFileName.DisplayOrder is null) return StatusCode(500);

                    ProductImageFileNameInfo? oldImageFileNameInCurrentProduct = _firstProduct.ImageFileNames.Find(
                        imgFileNameInfo => imgFileNameInfo.FileName == oldProductImageFileName.FileName);
                       
                    if (oldImageFileNameInCurrentProduct is not null) continue;

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return StatusCode(500);

                    oldFirstProduct.ImageFileNames.RemoveAt(i);

                    i--;

                    for (int k = 0; k < oldFirstProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldFirstProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
        }

        if (_firstProduct.ImageFileNames is not null)
        {
            int newImageFileNamesCount = 0;

            foreach (ProductImageFileNameInfo imageFileNameInfo in _firstProduct.ImageFileNames)
            {
                ProductImageFileNameInfo? imageFileNameOfOldProduct = oldFirstProduct.ImageFileNames?.Find(
                    imgFileNameInfo => imgFileNameInfo.FileName == imageFileNameInfo.FileName);

                if (imageFileNameOfOldProduct is null)
                {
                    ProductImage? imageRelatedToThatFileNameInfo = OrdersForImagesInFirstProduct.Find(
                        x => imageFileNameInfo.DisplayOrder == x.displayOrder)
                        .productImage;

                    if (imageRelatedToThatFileNameInfo is null) return StatusCode(500);

                    ServiceProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = new()
                    {
                        ProductId = _firstProduct.Id,
                        DisplayOrder = imageFileNameInfo.DisplayOrder,
                        Active = imageFileNameInfo.Active,
                        FileName = $"{imageRelatedToThatFileNameInfo.Id}.{GetFileExtensionFromFileType(imageRelatedToThatFileNameInfo.ImageContentType ?? "*")}",
                    };

                    imageFileNameInfoCreateRequests.Add(imageFileNameCreateRequest);

                    newImageFileNamesCount++;

                    continue;
                }

                if (imageFileNameInfo.FileName is null
                    || imageFileNameInfo.DisplayOrder is null) continue;

                int displayOrder = imageFileNameInfo.DisplayOrder.Value - newImageFileNamesCount;

                bool isFileNameInfoSameAsOldOne = displayOrder == imageFileNameOfOldProduct.DisplayOrder
                    && imageFileNameInfo.Active == imageFileNameOfOldProduct.Active;

                if (isFileNameInfoSameAsOldOne) continue;

                ServiceProductImageFileNameInfoByFileNameUpdateRequest imageFileNameUpdateRequest = new()
                {
                    ProductId = productId,
                    NewDisplayOrder = displayOrder,
                    Active = imageFileNameInfo.Active,
                    FileName = imageFileNameInfo.FileName,
                    NewFileName = null,
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> productImageFileNameUpdateResult
                    = _productImageFileNameInfoService.UpdateByFileName(imageFileNameUpdateRequest);

                IStatusCodeActionResult actionResultFromImageFileNameUpdate = productImageFileNameUpdateResult.Match<IStatusCodeActionResult>(
                    id => new OkResult(),
                    validationResult => BadRequest(validationResult),
                    unexpectedFailureResult => StatusCode(500));

                if (actionResultFromImageFileNameUpdate.StatusCode != 200)
                {
                    return actionResultFromImageFileNameUpdate;
                }
            }
        }

        IEnumerable<ServiceProductImageFileNameInfoCreateRequest> orderedImageFileNameInfoCreateRequests = imageFileNameInfoCreateRequests
            .OrderBy(imageFileName => imageFileName.DisplayOrder);

        foreach (ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest in orderedImageFileNameInfoCreateRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

            IStatusCodeActionResult actionResultFromImageFileNameInsert = fileNameInfoInsertResult.Match<IStatusCodeActionResult>(
                success => new OkResult(),
                validationResult => BadRequest(validationResult),
                unexpectedFailureResult => StatusCode(500));

            if (actionResultFromImageFileNameInsert.StatusCode != 200)
            {
                return actionResultFromImageFileNameInsert;
            }
        }

        return new OkResult();
    }

    private ProductFullEditorPartialModel GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum productIndicator)
    {
        if (productIndicator == FirstOrSecondProductEnum.First)
        {
            return new ProductFullEditorPartialModel()
            {
                Product = _firstProduct,
                XmlProduct = _firstProductXmlObjectData?.Products.FirstOrDefault(),
                ElementIdAndNamePrefix = "productCompareEditor_LocalProductEditor_",
                OtherElementIdAndNamePrefix = "productCompareEditor_OutsideProductEditor_",
                FirstOrSecondProduct = FirstOrSecondProductEnum.First,
                CharacteristicsRelatedToProduct = CharacteristicsRelatedToFirstProduct,
                ProductSearchStringPartsAndDataAboutTheirOrigin = FirstProductSearchStringPartsAndDataAboutTheirOrigin,
                ImagesContainerId = "productCompareEditor_LocalProductEditor_imageContainer_div",
                OtherImagesContainerId = "productCompareEditor_OutsideProductEditor_imageContainer_div",
                ValidationFormId = "productCompareEditor_LocalProductEditorContainer_validationForm",
            };
        }

        return new ProductFullEditorPartialModel()
        {
            Product = _secondProduct,
            XmlProduct = _secondProductXmlObjectData?.Products.FirstOrDefault(),
            ElementIdAndNamePrefix = "productCompareEditor_OutsideProductEditor_",
            OtherElementIdAndNamePrefix = "productCompareEditor_LocalProductEditor_",
            FirstOrSecondProduct = FirstOrSecondProductEnum.Second,
            CharacteristicsRelatedToProduct = CharacteristicsRelatedToSecondProduct,
            ProductSearchStringPartsAndDataAboutTheirOrigin = SecondProductSearchStringPartsAndDataAboutTheirOrigin,
            ImagesContainerId = "productCompareEditor_OutsideProductEditor_imageContainer_div",
            OtherImagesContainerId = "productCompareEditor_LocalProductEditor_imageContainer_div",
            ValidationFormId = null,
        };
    }

    public IActionResult OnPutClearSecondProductData()
    {
        _secondProduct = null;
        _secondProductXml = null;
        _secondProductSearchStringPartsAndDataAboutTheirOrigin = null;
        _secondProductXmlObjectData = null;
        _characteristicsRelatedToSecondProduct = null;

        return new OkResult();
    }

    public IActionResult OnPostAddNewImageToProductFirst(IFormFile fileInfo)
    {
        if (_firstProduct is null) return BadRequest();

        (ProductImage productImage, ProductImageFileNameInfo productImageFileNameInfo)
            = GetImageAndImageFileNameInfoFromFileAndProductData(_firstProduct, fileInfo);

        OrdersForImagesInFirstProduct.Add((productImage, productImageFileNameInfo.DisplayOrder ?? (_firstProduct.ImageFileNames?.Count ?? 0 + 1)));

        _firstProduct.Images ??= new();

        _firstProduct.Images.Add(productImage);

        _firstProduct.ImageFileNames ??= new();

        _firstProduct.ImageFileNames.Add(productImageFileNameInfo);

        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnPostAddNewImageToProductSecond(IFormFile fileInfo)
    {
        if (_secondProduct is null) return BadRequest();

        (ProductImage productImage, ProductImageFileNameInfo productImageFileNameInfo)
            = GetImageAndImageFileNameInfoFromFileAndProductData(_secondProduct, fileInfo);

        RelatedImagesAndFileInfosInSecondProduct.Add((productImage, productImageFileNameInfo));

        _secondProduct.Images ??= new();

        _secondProduct.Images.Add(productImage);

        _secondProduct.ImageFileNames ??= new();

        _secondProduct.ImageFileNames.Add(productImageFileNameInfo);

        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));
    }

    private (ProductImage newProductImage, ProductImageFileNameInfo newImageFileNameInfo)
        GetImageAndImageFileNameInfoFromFileAndProductData(Product product, IFormFile fileInfo)
    {
        using MemoryStream stream = new();

        fileInfo.CopyTo(stream);

        byte[] imageBytes = stream.ToArray();

        string contentType = fileInfo.ContentType;

        List<Product> products = new() { product };

        XmlObjectData xmlObjectData = _productXmlToCreateRequestMappingService.GetXmlDataFromProducts(products);

        string? productXml = _productDeserializeService.SerializeProductsXml(xmlObjectData, true, true);

        ProductImage image = new()
        {
            ImageData = imageBytes,
            ImageContentType = contentType,
            HtmlData = productXml,
        };

        ProductImageFileNameInfo imageFileNameInfo = new()
        {
            FileName = Guid.NewGuid().ToString(),
            DisplayOrder = product.ImageFileNames?.Count + 1 ?? 1,
            Active = true,
        };

        return (image, imageFileNameInfo);
    }

    public IActionResult OnPostAddNewImageAtGivenDisplayOrderToProductFirst(int displayOrder, IFormFile fileInfo)
    {
        if (_firstProduct is null
            || displayOrder <= 0) return BadRequest();

        OnPostAddNewImageToProductFirst(fileInfo);

        ProductImageFileNameInfo lastImageFileName = _firstProduct.ImageFileNames!.Last();

        if (lastImageFileName.DisplayOrder!.Value != displayOrder)
        {
            OnPutUpdateImageOrderForProductFirst((uint)lastImageFileName.DisplayOrder!.Value, (uint)displayOrder);
        }

        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnPostAddNewImageAtGivenDisplayOrderToProductSecond(int displayOrder, IFormFile fileInfo)
    {
        if (_secondProduct is null
            || displayOrder <= 0) return BadRequest();

        OnPostAddNewImageToProductSecond(fileInfo);

        ProductImageFileNameInfo lastImageFileName = _secondProduct.ImageFileNames!.Last();

        if (lastImageFileName.DisplayOrder!.Value != displayOrder)
        {
            OnPutUpdateImageOrderForProductSecond((uint)lastImageFileName.DisplayOrder!.Value, (uint)displayOrder);
        }

        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second));
    }

    public IActionResult OnPutUpdateImageOrderForProductFirst(uint oldDisplayOrder, uint newDisplayOrder)
    {
        if (_firstProduct is null) return BadRequest();

        bool success = UpdateImageFileNameInfoDisplayOrder(_firstProduct, oldDisplayOrder, newDisplayOrder);

        if (!success)
        {
            (ProductImage? productImage, int? displayOrder)
                = OrdersForImagesInFirstProduct.Find(x => x.displayOrder == oldDisplayOrder);

            if (productImage is null)
            {
                ProductImage? imageToMove = _firstProduct.Images?[(int)oldDisplayOrder - 1];

                if (imageToMove is null) return BadRequest();

                OrdersForImagesInFirstProduct.Add((imageToMove, (int)oldDisplayOrder));
            }
        }

        OrderImagesInProductByDisplayOrderForLastChange(_firstProduct, (int)oldDisplayOrder - 1, (int)newDisplayOrder - 1);

        for (int i = 0; i < OrdersForImagesInFirstProduct.Count; i++)
        {
            (ProductImage productImage, int displayOrder) = OrdersForImagesInFirstProduct[i];

            displayOrder = ChangeDisplayOrderBasedOnLastChange(oldDisplayOrder, newDisplayOrder, displayOrder);

            OrdersForImagesInFirstProduct[i] = new(productImage, displayOrder);
        }


        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    private static int ChangeDisplayOrderBasedOnLastChange(uint oldDisplayOrder, uint newDisplayOrder, int displayOrder)
    {
        if (displayOrder == oldDisplayOrder)
        {
            displayOrder = (int)newDisplayOrder;
        }
        else if (oldDisplayOrder < newDisplayOrder
            && displayOrder > oldDisplayOrder
            && displayOrder <= newDisplayOrder)
        {
            displayOrder--;
        }
        else if (oldDisplayOrder > newDisplayOrder
            && displayOrder < oldDisplayOrder
            && displayOrder >= newDisplayOrder)
        {
            displayOrder++;
        }

        return displayOrder;
    }

    public IActionResult OnPutUpdateImageOrderForProductSecond(uint oldDisplayOrder, uint newDisplayOrder)
    {
        if (_secondProduct is null) return BadRequest();

        bool success = UpdateImageFileNameInfoDisplayOrder(_secondProduct, oldDisplayOrder, newDisplayOrder);

        OrderImagesInProductByDisplayOrder(_secondProduct, RelatedImagesAndFileInfosInSecondProduct);

        return success ? Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.Second))
            : BadRequest();
    }

    private static bool UpdateImageFileNameInfoDisplayOrder(Product? product, uint oldDisplayOrder, uint newDisplayOrder)
    {
        if (product is null
            || product.ImageFileNames is null) return false;

        int oldDisplayOrderInt = (int)oldDisplayOrder;

        int indexOfItemToMove = product.ImageFileNames.FindIndex(x => x.DisplayOrder == oldDisplayOrderInt);

        if (indexOfItemToMove == -1) return false;

        ProductImageFileNameInfo? itemToMove = product.ImageFileNames.ElementAtOrDefault(indexOfItemToMove);

        if (itemToMove is null) return false;

        int newDisplayOrderInt = (int)newDisplayOrder;

        if (newDisplayOrderInt > product.ImageFileNames.Count)
        {
            newDisplayOrderInt = product.ImageFileNames.Count;
        }

        itemToMove.DisplayOrder = newDisplayOrderInt;

        product.ImageFileNames.RemoveAt(indexOfItemToMove);

        product.ImageFileNames.Insert(newDisplayOrderInt - 1, itemToMove);

        for (int i = 0; i < product.ImageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileNameInfo = product.ImageFileNames[i];

            imageFileNameInfo.DisplayOrder = i + 1;
        }

        return true;
    }

    private static void OrderImagesInProductByDisplayOrderForLastChange(
        Product product,
        int lastOldIndex,
        int lastNewIndex)
    {
        if (product.Images is null
            || product.Images.Count == 0) return;

        ProductImage? imageToMove = product.Images.ElementAtOrDefault(lastOldIndex);

        if (imageToMove is null) return;

        product.Images.RemoveAt(lastOldIndex);

        product.Images.Insert(lastNewIndex, imageToMove);
    }

    private static void OrderImagesInProductByDisplayOrder(Product product,
        List<(ProductImage productImage, ProductImageFileNameInfo imageFileNameInfo)> imagesAndImageFileNames)
    {
        product.Images = imagesAndImageFileNames
            .OrderBy(x => x.imageFileNameInfo.DisplayOrder)
            .Select(x => x.productImage)
            .ToList();
    }

    public IActionResult OnPutUpdateImageActiveStatusForProductFirst(uint displayOrder, bool active)
    {
        if (_firstProduct is null
            || _firstProduct.ImageFileNames is null) return BadRequest();

        int displayOrderInt = (int)displayOrder;

        bool success = UpdateImageActiveStatusForProduct(_firstProduct, displayOrderInt, active);

        return success ? new OkResult() : BadRequest();
    }

    public IActionResult OnPutUpdateImageActiveStatusForProductSecond(uint displayOrder, bool active)
    {
        if (_secondProduct is null
            || _secondProduct.ImageFileNames is null) return BadRequest();

        int displayOrderInt = (int)displayOrder;

        bool success = UpdateImageActiveStatusForProduct(_secondProduct, displayOrderInt, active);

        return success ? new OkResult() : BadRequest();
    }

    private static bool UpdateImageActiveStatusForProduct(Product product, int displayOrder, bool active)
    {
        if (product.ImageFileNames is null) return false;

        ProductImageFileNameInfo? item = product.ImageFileNames.Find(x => x.DisplayOrder == displayOrder);

        if (item is null) return false;

        item.Active = active;

        return true;
    }

    public IActionResult OnDeleteDeleteImageForProductFirst(uint imageId, string fileType, string? fileName = null)
    {
        if (_firstProduct is null
        || _firstProduct.ImageFileNames is null) return BadRequest();

        ProductImage? imageToRemove = _firstProduct.Images?.Find(x => x.Id == (int)imageId);

        if (imageToRemove is null) return BadRequest();

        string fileNameToSearchBy = fileName ?? $"{imageId}.{GetFileExtensionFromFileType(fileType)}";

        ProductImageFileNameInfo? productImageFileNameInfoToRemove = _firstProduct.ImageFileNames
            .Find(imageFileNameInfo => imageFileNameInfo.FileName == fileNameToSearchBy);

        IActionResult deleteImageResult = DeleteImageForProduct(_firstProduct, imageId, fileType, fileName);
        
        for (int i = 0; i < OrdersForImagesInFirstProduct.Count; i++)
        {
            (ProductImage productImage, int displayOrder) = OrdersForImagesInFirstProduct[i];

            if (productImage == imageToRemove)
            {
                OrdersForImagesInFirstProduct.RemoveAt(i);

                i--;
            }

            if (displayOrder > productImageFileNameInfoToRemove?.DisplayOrder)
            {
                displayOrder--;

                OrdersForImagesInFirstProduct[i] = new(productImage, displayOrder);
            }
        }

        return Partial("ProductCompareEditor/_ProductFullEditorImageDisplayPartial",
            GetProductFullEditorModelBasedOnProduct(FirstOrSecondProductEnum.First));
    }

    public IActionResult OnDeleteDeleteImageForProductSecond(uint imageId, string fileType, string? fileName = null)
    {
        if (_secondProduct is null
            || _secondProduct.ImageFileNames is null) return BadRequest();

        return DeleteImageForProduct(_secondProduct, imageId, fileType, fileName);
    }

    private IActionResult DeleteImageForProduct(Product product, uint imageId, string fileType, string? fileName)
    {
        if (product.ImageFileNames is null) return BadRequest();

        int indexOfImageToRemove = product.Images?.FindIndex(x => x.Id == (int)imageId) ?? -1;

        if (indexOfImageToRemove == -1) return BadRequest();

        product.Images!.RemoveAt(indexOfImageToRemove);

        string fileNameToSearchBy = fileName ?? $"{imageId}.{GetFileExtensionFromFileType(fileType)}";

        int indexOfImageFileNameInfoToRemove = product.ImageFileNames
            .FindIndex(imageFileNameInfo => imageFileNameInfo.FileName == fileNameToSearchBy);

        if (indexOfImageFileNameInfoToRemove != -1)
        {
            product.ImageFileNames.RemoveAt(indexOfImageFileNameInfoToRemove);

            for (int i = 0; i < product.ImageFileNames.Count; i++)
            {
                ProductImageFileNameInfo imageFileNameInfo = product.ImageFileNames[i];

                imageFileNameInfo.DisplayOrder = i + 1;
            }
        }

        return new OkResult();
    }

    public IActionResult OnGetGetAllowedCharacteristicIdsFirst()
    {
        return GetAllowedCharacteristicIds(FirstOrSecondProductEnum.First);
    }

    public IActionResult OnGetGetAllowedCharacteristicIdsSecond()
    {
        return GetAllowedCharacteristicIds(FirstOrSecondProductEnum.Second);
    }

    private static JsonResult GetAllowedCharacteristicIds(FirstOrSecondProductEnum firstOrSecondProductEnum)
    {
        IEnumerable<int>? allowedIds = firstOrSecondProductEnum switch
        {
            FirstOrSecondProductEnum.First => _characteristicsRelatedToFirstProduct?.Select(x => x.Id),
            FirstOrSecondProductEnum.Second => _characteristicsRelatedToSecondProduct?.Select(x => x.Id),
            _ => throw new InvalidOperationException("No such case exists")
        };

        return new JsonResult(new { allowedIds });
    }

    public IActionResult OnGetGetRemainingCharacteristicsForProductFirst(int? characteristicToAddToSelectId = null)
    {
        if (characteristicToAddToSelectId is not null
            && characteristicToAddToSelectId < 0) return BadRequest();

        IEnumerable<SelectListItem> remainingCharacteristics
            = GetRemainingCharacteristicsForProduct(_firstProduct, _characteristicsRelatedToFirstProduct, false);

        if (characteristicToAddToSelectId is not null)
        {
            ProductCharacteristic? characteristicToAdd = _characteristicsRelatedToFirstProduct?.Find(
                characteristic => characteristic.Id == characteristicToAddToSelectId);

            if (characteristicToAdd is null) return BadRequest();

            SelectListItem selectListItemForCharacteristic = new(characteristicToAdd.Name, characteristicToAdd.Id.ToString());

            remainingCharacteristics = remainingCharacteristics.Prepend(selectListItemForCharacteristic);
        }

        return new JsonResult(remainingCharacteristics.Select(
            selectListItem => new { text = selectListItem.Text, value = selectListItem.Value }));
    }

    public IActionResult OnGetGetRemainingCharacteristicsForProductSecond(int? characteristicToAddToSelectId = null)
    {
        if (characteristicToAddToSelectId is not null
            && characteristicToAddToSelectId < 0) return BadRequest();

        IEnumerable<SelectListItem> remainingCharacteristics
            = GetRemainingCharacteristicsForProduct(_secondProduct, _characteristicsRelatedToSecondProduct, false);

        if (characteristicToAddToSelectId is not null)
        {
            ProductCharacteristic? characteristicToAdd = _characteristicsRelatedToSecondProduct?.Find(
                characteristic => characteristic.Id == characteristicToAddToSelectId);

            if (characteristicToAdd is null) return BadRequest();

            SelectListItem selectListItemForCharacteristic = new(characteristicToAdd.Name, characteristicToAdd.Id.ToString());

            remainingCharacteristics = remainingCharacteristics.Prepend(selectListItemForCharacteristic);
        }

        return new JsonResult(remainingCharacteristics.Select(
            selectListItem => new { text = selectListItem.Text, value = selectListItem.Value }));
    }

    public static IEnumerable<SelectListItem> GetRemainingCharacteristicsForProduct(
        Product? product,
        List<ProductCharacteristic>? characteristicsRelatedToProduct,
        bool addFirstSelectCharacteristic)
    {
        if (product is null) return Enumerable.Empty<SelectListItem>();

        if (characteristicsRelatedToProduct is null
            || characteristicsRelatedToProduct.Count <= 0) return Enumerable.Empty<SelectListItem>();

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            IEnumerable<ProductCharacteristic> relevantCharacteristics = characteristicsRelatedToProduct
                .Where(characteristic => characteristic.KWPrCh != ProductCharacteristicTypeEnum.SearchStringAbbreviation);

            return ConvertCharacteristicsToSelectListItems(relevantCharacteristics, addFirstSelectCharacteristic);
        }

        IEnumerable<ProductCharacteristic> relevantCharacteristicsWithNoMatchesInProduct = characteristicsRelatedToProduct
            .Where(characteristic =>
            {
                if (characteristic.KWPrCh == ProductCharacteristicTypeEnum.SearchStringAbbreviation) return false;

                return product.Properties.Find(
                    property => property.ProductCharacteristicId == characteristic.Id) == null;
            });

        return ConvertCharacteristicsToSelectListItems(
            relevantCharacteristicsWithNoMatchesInProduct,
            addFirstSelectCharacteristic);
    }

    public IActionResult OnGetGetCharacteristicsForBothProducts()
    {
        IEnumerable<ProductCharacteristic>? characteristics = _characteristicsRelatedToFirstProduct;

        if (_characteristicsRelatedToSecondProduct is not null)
        {
            if (characteristics is not null)
            {
                characteristics = characteristics
                    .Concat(_characteristicsRelatedToSecondProduct);
            }
            else
            {
                characteristics = _characteristicsRelatedToSecondProduct;
            }
        }

        IEnumerable<ProductCharacteristic>? uniqueCharcteristics = characteristics?
            .DistinctBy(characteristic => characteristic.Id);

        return new JsonResult(new
        {
            characteristics = uniqueCharcteristics?
                .Select(characteristic => new
                {
                    text = characteristic.Name,
                    meaning = characteristic.Meaning,
                    value = characteristic.Id
                }
            )
        });
    }

    private static IEnumerable<SelectListItem> ConvertCharacteristicsToSelectListItems(
        IEnumerable<ProductCharacteristic> characteristicsToConvert,
        bool addFirstSelectCharacteristic)
    {
        IEnumerable<SelectListItem> convertedCharacteristics = characteristicsToConvert
            .Select(characteristic => new SelectListItem(characteristic.Name, characteristic.Id.ToString()));

        if (!addFirstSelectCharacteristic) return convertedCharacteristics;

        return convertedCharacteristics.Prepend(new SelectListItem("--- Select ---", null, true, true));
    }

    public IActionResult OnPostAddNewBlancPropertyToProductFirst(int indexOfItem, string elementIdPrefix)
    {
        return AddNewBlancProperty(_firstProduct, _characteristicsRelatedToFirstProduct, indexOfItem, elementIdPrefix, FirstOrSecondProductEnum.First);
    }

    public IActionResult OnPostAddNewBlancPropertyToProductSecond(int indexOfItem, string elementIdPrefix)
    {
        return AddNewBlancProperty(_secondProduct, _characteristicsRelatedToSecondProduct, indexOfItem, elementIdPrefix, FirstOrSecondProductEnum.Second);
    }

    public IActionResult AddNewBlancProperty(
        Product? product,
        List<ProductCharacteristic>? characteristicsRelatedToProduct,
        int indexOfItem,
        string elementIdPrefix,
        FirstOrSecondProductEnum firstOrSecondProductEnum)
    {
        if (product == null
            || indexOfItem < 0) return BadRequest();

        List<SelectListItem> relatedCharacteristics = GetRemainingCharacteristicsForProduct(product, characteristicsRelatedToProduct, false)
            .ToList();

        SelectListItem? firstCharacteristic = relatedCharacteristics[0];

        int? characteristicId = null;

        if (firstCharacteristic is not null)
        {
            bool parseIdSuccess = int.TryParse(firstCharacteristic.Value, out int firstCharacteristicId);

            characteristicId = parseIdSuccess ? firstCharacteristicId : null;

            product.Properties.Add(new()
            {
                ProductCharacteristicId = characteristicId,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList
            });
        }

        return Partial("ProductCompareEditor/ProductProperties/_ProductPropertyInCompareEditorWithoutCharacteristicPartial",
            new ProductPropertyInCompareEditorWithoutCharacteristicPartialModel()
            {
                ProductProperty = new ProductProperty() { ProductCharacteristicId = characteristicId, XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                ProductCharacteristicsForSelect = relatedCharacteristics,
                PropertyIndex = (uint)indexOfItem,
                ElementIdAndNamePrefix = elementIdPrefix,
                ProductFirstOrSecondEnum = firstOrSecondProductEnum,
                ValidationFormId = (firstOrSecondProductEnum == FirstOrSecondProductEnum.First) ? "productCompareEditor_LocalProductEditorContainer_validationForm" : null
            });
    }

    public IActionResult OnPutAddOrOverwritePropertyFromDataInOtherProductFirst(
        int characteristicId,
        int indexToInsertAtOnAdd,
        XMLPlacementEnum xmlPlacementValue)
    {
        if (_firstProduct is null
            || _characteristicsRelatedToFirstProduct is null
            || _secondProduct is null) return BadRequest();

        return AddOrOverwritePropertyFromDataInOtherProduct(
            _firstProduct,
            FirstOrSecondProductEnum.First,
            _characteristicsRelatedToFirstProduct,
            _secondProduct,
            characteristicId,
            indexToInsertAtOnAdd,
            xmlPlacementValue);
    }

    public IActionResult OnPutAddOrOverwritePropertyDataInFromOtherProductSecond(
        int characteristicId,
        int indexToInsertAtOnAdd,
        XMLPlacementEnum xmlPlacementValue)
    {
        if (_secondProduct is null
            || _characteristicsRelatedToSecondProduct is null
            || _firstProduct is null) return BadRequest();

        return AddOrOverwritePropertyFromDataInOtherProduct(
            _secondProduct,
            FirstOrSecondProductEnum.Second,
            _characteristicsRelatedToSecondProduct,
            _firstProduct,
            characteristicId,
            indexToInsertAtOnAdd,
            xmlPlacementValue);
    }

    private IActionResult AddOrOverwritePropertyFromDataInOtherProduct(
        Product product,
        FirstOrSecondProductEnum firstOrSecondProductEnum,
        List<ProductCharacteristic> characteristicsRelatedToProduct,
        Product otherProduct,
        int characteristicId,
        int indexToInsertAtOnAdd,
        XMLPlacementEnum xmlPlacementValue)
    {
        if (product is null
            || characteristicId < 0
            || indexToInsertAtOnAdd < 0) return BadRequest();

        ProductCharacteristic? productCharacteristic
            = characteristicsRelatedToProduct.Find(characteristic => characteristic.Id == characteristicId);

        if (productCharacteristic is null) return BadRequest();

        ProductProperty? otherProductProperty = otherProduct.Properties.Find(prop => 
            prop.ProductCharacteristicId == characteristicId);

        if (otherProductProperty is null) return BadRequest();

        ProductProperty copyOfOtherProductProperty = new()
        {
            ProductId = product.Id,
            ProductCharacteristicId = otherProductProperty.ProductCharacteristicId,
            Characteristic = otherProductProperty.Characteristic,
            Value = otherProductProperty.Value,
            DisplayOrder = otherProductProperty.DisplayOrder,
            XmlPlacement = xmlPlacementValue,
        };

        int indexOfPropertyWithSameCharacteristicInProduct = product.Properties.FindIndex(
            prop => prop.ProductCharacteristicId == characteristicId);

        if (indexOfPropertyWithSameCharacteristicInProduct > -1)
        {
            product.Properties[indexOfPropertyWithSameCharacteristicInProduct] = copyOfOtherProductProperty;

            return Partial("ProductCompareEditor/_ProductFullEditorPartial",
                GetProductFullEditorModelBasedOnProduct(firstOrSecondProductEnum));
        }

        product.Properties.Insert(indexToInsertAtOnAdd, copyOfOtherProductProperty);

        return Partial("ProductCompareEditor/_ProductFullEditorPartial",
            GetProductFullEditorModelBasedOnProduct(firstOrSecondProductEnum));
    }

    public IActionResult OnPutUpdatePropertyForProductFirst([FromBody] ProductCompareEditorPropertyData data, int? productCharacteristicToRemoveId = null)
    {
        bool success = UpdatePropertyForProduct(_firstProduct, _characteristicsRelatedToFirstProduct, data, productCharacteristicToRemoveId);

        return success ? new OkResult() : BadRequest();
    }

    public IActionResult OnPutUpdatePropertyForProductSecond([FromBody] ProductCompareEditorPropertyData data, int? productCharacteristicToRemoveId = null)
    {
        bool success = UpdatePropertyForProduct(_secondProduct, _characteristicsRelatedToSecondProduct, data, productCharacteristicToRemoveId);

        return success ? new OkResult() : BadRequest();
    }

    private static bool UpdatePropertyForProduct(
        Product? product,
        List<ProductCharacteristic>? characteristicsRelatedToProduct,
        ProductCompareEditorPropertyData data,
        int? productCharacteristicToRemoveId = null)
    {
        if (data is null
            || product is null
            || characteristicsRelatedToProduct is null
            || data.ProductCharacteristicId == 0) return false;

        product.Properties ??= new();

        if (productCharacteristicToRemoveId is not null)
        {
            if (productCharacteristicToRemoveId < 0) return false;

            int indexOfPropToRemove = product.Properties.FindIndex(property =>
                property.ProductCharacteristicId == productCharacteristicToRemoveId);

            if (indexOfPropToRemove > -1)
            {
                product.Properties.RemoveAt(indexOfPropToRemove);
            }
        }

        int productPropertyForSameCharacteristicInProductIndex = product.Properties
            .FindIndex(property => property.ProductCharacteristicId == data.ProductCharacteristicId);

        ProductProperty? oldProductPropertyForSameCharacteristic = null;

        if (productPropertyForSameCharacteristicInProductIndex != -1)
        {
            oldProductPropertyForSameCharacteristic = product.Properties[productPropertyForSameCharacteristicInProductIndex];
        }

        ProductCharacteristic? characteristicForProperty = characteristicsRelatedToProduct.Find(characteristic =>
            characteristic.Id == data.ProductCharacteristicId);

        ProductProperty productProperty = new()
        {
            ProductId = product.Id,
            ProductCharacteristicId = data.ProductCharacteristicId,
            XmlPlacement = data.XmlPlacement ?? oldProductPropertyForSameCharacteristic?.XmlPlacement,
            Value = data.Value ?? oldProductPropertyForSameCharacteristic?.Value,
            DisplayOrder = null,
            Characteristic = characteristicForProperty?.Name,
        };

        if (data.NameOfCharacteristicToReplace is not null)
        {
            data.NameOfCharacteristicToReplace = HttpUtility.HtmlDecode(data.NameOfCharacteristicToReplace);

            int indexOfCharacteristicToReplace = product.Properties.FindIndex(property =>
                property.Characteristic == data.NameOfCharacteristicToReplace);

            if (indexOfCharacteristicToReplace > -1)
            {
                product.Properties[indexOfCharacteristicToReplace] = productProperty;

                return true;
            }
        }

        if (data.IsNew)
        {
            product.Properties.Add(productProperty);

            return true;
        }

        if (productPropertyForSameCharacteristicInProductIndex == -1)
        {
            product.Properties.Add(productProperty);

            return true;
        }

        product.Properties[productPropertyForSameCharacteristicInProductIndex] = productProperty;

        return true;
    }

    public IActionResult OnPutUpdatePropertyCharacteristicIdFirst(int oldCharacteristicId, int newCharacteristicId)
    {
        if (_firstProduct is null) return BadRequest();

        return UpdatePropertyCharacteristicId(_firstProduct, _characteristicsRelatedToFirstProduct, oldCharacteristicId, newCharacteristicId);
    }

    public IActionResult OnPutUpdatePropertyCharacteristicIdSecond(int oldCharacteristicId, int newCharacteristicId)
    {
        if (_secondProduct is null) return BadRequest();

        return UpdatePropertyCharacteristicId(_secondProduct, _characteristicsRelatedToSecondProduct, oldCharacteristicId, newCharacteristicId);
    }

    public IActionResult UpdatePropertyCharacteristicId(
        Product product,
        List<ProductCharacteristic>? characteristicsRelatedToProduct,
        int oldCharacteristicId,
        int newCharacteristicId)
    {
        if (oldCharacteristicId < 0
            || newCharacteristicId < 0) return BadRequest();

        ProductProperty? propWithOldCharacteristicId = product.Properties.Find(prop =>
            prop.ProductCharacteristicId == oldCharacteristicId);

        if (propWithOldCharacteristicId is null) return BadRequest();

        ProductCharacteristic? relatedNewCharacteristic = characteristicsRelatedToProduct?.Find(
            characteristic => characteristic.Id == newCharacteristicId);

        if (relatedNewCharacteristic is null) return BadRequest();

        propWithOldCharacteristicId.ProductCharacteristicId = newCharacteristicId;
        propWithOldCharacteristicId.Characteristic = relatedNewCharacteristic.Name;

        return new OkResult();
    }

    public IActionResult OnDeleteDeletePropertyForProductFirst(int productCharacteristicId)
    {
        bool success = DeletePropertyFromProduct(_firstProduct, productCharacteristicId);

        return success ? new OkResult() : BadRequest();
    }

    public IActionResult OnDeleteDeletePropertyForProductSecond(int productCharacteristicId)
    {
        bool success = DeletePropertyFromProduct(_secondProduct, productCharacteristicId);

        return success ? new OkResult() : BadRequest();
    }

    private static bool DeletePropertyFromProduct(Product? product, int productCharacteristicId)
    {
        if (product is null
            || productCharacteristicId <= 0) return false;

        int indexOfPropertyToDelete = product.Properties
            .FindIndex(property => property.ProductCharacteristicId == productCharacteristicId);

        if (indexOfPropertyToDelete == -1)
        {
            return false;
        }

        product.Properties.RemoveAt(indexOfPropertyToDelete);

        return true;
    }

    private static string GetFileExtensionFromFileType(string fileType)
    {
        if (fileType.StartsWith("image/"))
        {
            return fileType[(fileType.IndexOf('/') + 1)..];
        }

        return fileType;
    }

    private static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.SequenceEqual(b);
    }

    private static Product? CloneProduct(Product? product)
    {
        if (product is null) return null;

        return new Product()
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
            Price = product.Price,
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
            CategoryID = product.CategoryID,
            Category = product.Category,
            ManifacturerId = product.ManifacturerId,
            Manifacturer = product.Manifacturer,
            SubCategoryId = product.SubCategoryId,
            Properties = product.Properties
                .Select(x => new ProductProperty()
                {
                    ProductId = x.ProductId,
                    ProductCharacteristicId = x.ProductCharacteristicId,
                    Characteristic = x.Characteristic,
                    XmlPlacement = x.XmlPlacement,
                    DisplayOrder = x.DisplayOrder,
                    Value = x.Value,
                })
                .ToList(),

            Images = product.Images?
                .Select(x => new ProductImage()
                {
                    Id = x.Id,
                    ImageData = x.ImageData,
                    ImageContentType = x.ImageContentType,
                    ProductId = x.ProductId,
                    HtmlData = x.HtmlData,
                    DateModified = x.DateModified,
                })
                .ToList(),

            ImageFileNames = product.ImageFileNames?
                .Select(x => new ProductImageFileNameInfo()
                {
                    ProductId = x.ProductId,
                    DisplayOrder = x.DisplayOrder,
                    FileName = x.FileName,
                    Active = x.Active,
                })
                .ToList(),
        };
    }

    public enum FirstOrSecondProductEnum
    {
        First = 0,
        Second = 1,
    }
}