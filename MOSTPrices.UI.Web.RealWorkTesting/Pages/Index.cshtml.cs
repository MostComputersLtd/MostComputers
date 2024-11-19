using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Utils.ProductImageFileNameUtils;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;
using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageAndFileNameRelationsUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductToDisplayDataMappingUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductMappingUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ImageMappingUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public IndexModel(IProductService productService,
        ICategoryService categoryService,
        IManifacturerService manifacturerService,
        IProductTableDataService productTableDataService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductWorkStatusesService productWorkStatusesService,
        ISearchStringOriginService searchStringOriginService,
        IProductDeserializeService productDeserializeService,
        IProductHtmlService productHtmlService,
        ILocalChangesCheckingImmediateExecutionService localChangesCheckingImmediateExecutionService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _manifacturerService = manifacturerService;
        _productTableDataService = productTableDataService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productWorkStatusesService = productWorkStatusesService;
        _searchStringOriginService = searchStringOriginService;
        _productDeserializeService = productDeserializeService;
        _productHtmlService = productHtmlService;
        _localChangesCheckingImmediateExecutionService = localChangesCheckingImmediateExecutionService;
        _productImageFileManagementService = productImageFileManagementService;

        _productTableDataService.PopulateIfEmptyAndGet(() => PopulateProductData(productTableDataService));

        Products = _productTableDataService.GetProductDataToDisplay();
    }

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IManifacturerService _manifacturerService;
    private readonly IProductTableDataService _productTableDataService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ILocalChangesCheckingImmediateExecutionService _localChangesCheckingImmediateExecutionService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;

    public IReadOnlyList<ProductDisplayData> Products { get; set; }

    private ProductDisplayData GetFullProductAndMapToDisplayData(Product product)
    {
        product.Images = _productImageService.GetAllInProduct(product.Id)
            .ToList();

        product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(product.Id)
            .ToList();

        product.Properties = _productPropertyService.GetAllInProduct(product.Id)
            .ToList();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(product.Id);

        ProductDisplayData productDisplayData = MapToProductDisplayData(product, productWorkStatuses);

        AddImagesFromFilesToProductData(productDisplayData);

        return productDisplayData;
    }

    private List<ProductDisplayData> PopulateProductData(IProductTableDataService productTableDataService)
    {
        List<ProductDisplayData> output = new();

        Product? productWithHighestId = _productService.GetProductWithHighestId();

        if (productWithHighestId is not null)
        {
            ProductDisplayData productDisplayData = GetFullProductAndMapToDisplayData(productWithHighestId);

            output.Add(productDisplayData);

            productTableDataService.AddProductDataOrMakeExistingOneDisplayable(productDisplayData);
        }

        return output;
    }

    public int? GetProductHighestId()
    {
        Product? productWithHighestId = _productService.GetProductWithHighestId();

        return productWithHighestId?.Id;
    }

    public void OnGet()
    {
    }

    public IActionResult OnGetGetProductFirstImagePopupPartialViewForProduct(int productId)
    {
        if (productId < 0)
        {
            return BadRequest("Id cannot be negative");
        }

        ProductDisplayData? productDataToDisplay = _productTableDataService.ProductData.FirstOrDefault(product => product.Id == productId);

        if (productDataToDisplay is null)
        {
            return NotFound();
        }

        return Partial("ProductPopups/_ProductFirstImageDisplayPopupPartial", new ProductFirstImageDisplayPopupPartialModel(
            ProductImagePopupUsageEnum.ImagesInDatabase,
            productDataToDisplay,
            _productImageService,
            _productImageFileNameInfoService,
            _productImageFileManagementService,
            "topNotificationBox"));
    }

    public IActionResult OnGetGetPartialViewImagesForProduct(int productId, ProductImagePopupUsageEnum productImagePopupUsage = ProductImagePopupUsageEnum.DisplayData)
    {
        if (productId < 0)
        {
            return BadRequest("Id cannot be negative");
        }

        ProductDisplayData? productDataToDisplay = _productTableDataService.GetProductById(productId);

        if (productDataToDisplay is null)
        {
            return NotFound();
        }

        return Partial("ProductPopups/_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(
            productImagePopupUsage,
            productDataToDisplay,
            _productImageService,
            _productImageFileNameInfoService,
            _productImageFileManagementService,
            "ProductImages_popup_modal_content",
            "topNotificationBox"));
    }

    public IActionResult OnGetGetProductChangesPopupPartialViewForProduct(int productId, bool getEvenIfProductDoesntExist = false)
    {
        if (productId < 0)
        {
            return BadRequest("Id cannot be negative");
        }

        Product? productToDisplay = _productService.GetProductFull(productId);

        if (productToDisplay is null)
        {
            if (getEvenIfProductDoesntExist)
            {
                return Partial("ProductPopups/_ProductChangesPopupPartial",
                    new ProductChangesPopupPartialModel(null, null));
            }

            return NotFound();
        }

        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringOriginData
            = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(productToDisplay);

        return Partial("ProductPopups/_ProductChangesPopupPartial", new ProductChangesPopupPartialModel(productToDisplay, searchStringOriginData));
    }

    public IActionResult OnGetGetProductFullPopupPartialViewForProduct(int productId)
    {
        Product? productToDisplay = _productService.GetProductFull(productId);

        if (productToDisplay is null) return NotFound();

        OneOf<string, InvalidXmlResult> productXmlResult = _productDeserializeService.TrySerializeProductXml(productToDisplay, true, true);

        string productXml = productXmlResult.Match(
            xml => xml ?? string.Empty,
            invalidXmlResult => string.Empty);

        string productManufacturerSiteLink = string.Empty;

        return Partial("ProductPopups/_ProductFullDisplayWithXmlPopupPartial",
            new ProductFullDisplayWithXmlPopupPartialModel(productToDisplay, productXml, productManufacturerSiteLink));
    }

    public IActionResult OnGetGetProductFullHtmlBasedPopupPartialViewForProduct(int productId, bool getEvenIfProductDoesntExist = false)
    {
        if (productId < 0)
        {
            return BadRequest("Id cannot be negative");
        }

        Product? productToDisplay = _productService.GetProductFull(productId);

        if (productToDisplay is null)
        {
            if (getEvenIfProductDoesntExist)
            {
                return Partial("ProductPopups/_ProductFullHtmlBasedDisplayPopupPartial",
                    new ProductFullHtmlBasedDisplayPopupPartialModel()
                    {
                        ProductHtmlService = _productHtmlService
                    });
            }

            return NotFound();
        }

        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringOriginData
            = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(productToDisplay);

        return Partial("ProductPopups/_ProductFullHtmlBasedDisplayPopupPartial", new ProductFullHtmlBasedDisplayPopupPartialModel()
        {
            ProductHtmlService = _productHtmlService,
            Product = productToDisplay,
            ProductSearchStringPartsAndDataAboutTheirOrigin = searchStringOriginData
        });
    }

    public IActionResult OnGetXml(int productId)
    {
        Product? productToDisplay = _productService.GetProductFull(productId);

        if (productToDisplay is null) return NotFound();

        OneOf<string, InvalidXmlResult> productXmlResult = _productDeserializeService.TrySerializeProductXml(productToDisplay, true, true);

        return productXmlResult.Match<IStatusCodeActionResult>(
            productXml => new OkObjectResult(productXml),
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    public async Task<IActionResult> OnPostAddNewProductWithExistingProductDataAsync(
        int productToCopyFromId, int highestId, string htmlElementId, int tableIndex, [FromBody] ProductDisplayData productDisplayData)
    {
        if (productToCopyFromId <= 0) return BadRequest();

        ProductDisplayData? productDisplayDataOfProduct = GetMostRecentProductDisplayData(productToCopyFromId, productDisplayData);

        if (productDisplayDataOfProduct is null) return BadRequest();

        int newProductId = highestId + 1;

        productDisplayDataOfProduct.Id = newProductId;

        List<ImageFileAndFileNameInfoUpsertRequest> imageFileUpsertRequests = new();

        if (productDisplayDataOfProduct.ImagesAndImageFileInfos is not null)
        {
            for (int i = 0; i < productDisplayDataOfProduct.ImagesAndImageFileInfos.Count; i++)
            {
                ImageAndImageFileNameRelation imageAndImageFileNameRelation = productDisplayDataOfProduct.ImagesAndImageFileInfos[i];

                ImageFileAndFileNameInfoUpsertRequest? imageFileUpsertRequest
                    = GetFileUpsertRequest(imageAndImageFileNameRelation, false, false);

                if (imageFileUpsertRequest is null) return BadRequest();

                imageFileUpsertRequests.Add(imageFileUpsertRequest);
            }
        }

        ProductCreateWithoutImagesInDatabaseRequest productCreateRequestWithoutImagesInDB
            = MapToProductCreateRequestWithoutImagesInDB(productDisplayDataOfProduct, imageFileUpsertRequests);

        OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> insertFullWithImagesOnlyInDirectoryResult
            = await _productService.InsertWithImagesOnlyInDirectoryAsync(productCreateRequestWithoutImagesInDB);

        IStatusCodeActionResult insertFullWithImagesOnlyInDirectoryActionResult = insertFullWithImagesOnlyInDirectoryResult.Match(
            id =>
            {
                newProductId = id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500),
            directoryNotFoundResult => NotFound(),
            fileDoesntExistResult => NotFound(fileDoesntExistResult));

        if (insertFullWithImagesOnlyInDirectoryActionResult.StatusCode != 200) return insertFullWithImagesOnlyInDirectoryActionResult;

        Product? newProduct = _productService.GetProductFull(newProductId)!;

        ProductDisplayData? productDisplayDataInEditor = _productTableDataService.GetProductById(newProductId);

        if (productDisplayDataInEditor == null)
        {
            productDisplayDataInEditor = MapToProductDisplayData(newProduct!);

            _productTableDataService.AddProductDataOrMakeExistingOneDisplayable(productDisplayDataInEditor, true);
        }

        productDisplayDataInEditor.ProductNewStatus = ProductNewStatusEnum.New;
        productDisplayDataInEditor.ProductXmlStatus = ProductXmlStatusEnum.NotReady;
        productDisplayDataInEditor.ReadyForImageInsert = false;

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayDataInEditor);
    }

    private ProductDisplayData? GetMostRecentProductDisplayData(int productToCopyFromId, ProductDisplayData? productDisplayData)
    {
        ProductDisplayData? savedProductDisplayData = _productTableDataService.GetProductById(productToCopyFromId);

        if (savedProductDisplayData is null)
        {
            Product? product = _productService.GetProductFull(productToCopyFromId);

            if (product is null) return null;

            savedProductDisplayData = MapToProductDisplayData(product);
        }

        ProductDisplayData? productDisplayDataOfProduct = (productDisplayData is not null) ? productDisplayData : savedProductDisplayData;

        productDisplayDataOfProduct.ImagesAndImageFileInfos = savedProductDisplayData.ImagesAndImageFileInfos?.ToList();

        productDisplayDataOfProduct.Properties = savedProductDisplayData.Properties?.ToList();

        return CloneProductDisplayData(productDisplayDataOfProduct);
    }

    private ProductDisplayData GetMostRecentProductDisplayData(int productToCopyFromId, ProductDisplayData? productDisplayData, Product productFull)
    {
        ProductDisplayData? savedProductDisplayData = _productTableDataService.GetProductById(productToCopyFromId);

        savedProductDisplayData ??= MapToProductDisplayData(productFull);

        ProductDisplayData productDisplayDataOfProduct = (productDisplayData is not null) ? productDisplayData : savedProductDisplayData;

        productDisplayDataOfProduct.ImagesAndImageFileInfos = savedProductDisplayData.ImagesAndImageFileInfos?.ToList();

        productDisplayDataOfProduct.Properties = savedProductDisplayData.Properties?.ToList();

        return CloneProductDisplayData(productDisplayDataOfProduct);
    }

    public async Task<IActionResult> OnPostGetOnlyProductWithHighestIdAsync(string htmlElementId, int tableIndex)
    {
        Product? product = _productService.GetProductWithHighestId();

        if (product is null) return NotFound();

        product = _productService.GetProductFull(product.Id)!;

        ProductDisplayData productData = MapToProductDisplayData(product);

        OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addImageFilesResult
            = await AddImagesFromFilesToProductDataAsync(productData);

        return addImageFilesResult.Match<IStatusCodeActionResult>(
            isSuccessful =>
            {
                if (!isSuccessful) return BadRequest();

                _productTableDataService.RemoveAllIdsFromIdsToDisplay();

                _productTableDataService.AddProductDataOrMakeExistingOneDisplayable(productData);

                return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productData);
            },
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            notSupportedFileTypeResult => BadRequest());
    }

    public async Task<IActionResult> OnPostGetOnlyProductWithIdAsync(int productId, string htmlElementId, int tableIndex)
    {
        if (productId < 0) return BadRequest();

        Product? product = _productService.GetProductFull(productId);

        if (product is null) return NotFound();

        ProductDisplayData productDisplayData = MapToProductDisplayData(product);

        OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addImagesFromFilesResult
            = await AddImagesFromFilesToProductDataAsync(productDisplayData);

        return addImagesFromFilesResult.Match<IStatusCodeActionResult>(
            isSuccessful =>
            {
                if (!isSuccessful) return BadRequest();

                _productTableDataService.RemoveAllIdsFromIdsToDisplay();

                _productTableDataService.AddProductDataOrMakeExistingOneDisplayable(productDisplayData);

                return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayData);
            },
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            notSupportedFileTypeResult => BadRequest());
    }

    private PartialViewResult GetPartialTableRowFromProduct(
        string htmlElementId, int tableIndex, ProductDisplayData productDisplayData)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        IndexProductTableRowPartialModel productPartialModel = new(
            productDisplayData,
            htmlElementId,
            tableIndex,
            allPossibleCategories,
            allPossibleManifacturers,
            "ProductFullWithXml_popup_modal_content",
            "ProductFullHtmlBased_popup_modal_content",
            "ProductChanges_popup_modal_content",
            "ProductImages_popup_modal_content",
            "ProductFirstImage_popup_modal_content");

        return base.Partial("_IndexProductTableRowPartial", productPartialModel);
    }

    public IndexProductTableRowPartialModel GetTableRowModel(Product product, int tableIndex, string htmlElementId)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(product.Id);

        return new IndexProductTableRowPartialModel(
            MapToProductDisplayData(product, productWorkStatuses),
            htmlElementId,
            tableIndex,
            allPossibleCategories,
            allPossibleManifacturers,
            "ProductFullWithXml_popup_modal_content",
            "ProductFullHtmlBased_popup_modal_content",
            "ProductChanges_popup_modal_content",
            "ProductImages_popup_modal_content",
            "ProductFirstImage_popup_modal_content");
    }

    public IndexProductTableRowPartialModel GetTableRowModel(ProductDisplayData productData, int tableIndex, string htmlElementId)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productData.Id);

        if (productWorkStatuses is not null)
        {
            productData.ProductWorkStatusesId = productWorkStatuses.Id;
            productData.ProductNewStatus = productWorkStatuses.ProductNewStatus;
            productData.ProductXmlStatus = productWorkStatuses.ProductXmlStatus;
            productData.ReadyForImageInsert = productWorkStatuses.ReadyForImageInsert;
        }

        return new IndexProductTableRowPartialModel(
            productData,
            htmlElementId,
            tableIndex,
            allPossibleCategories,
            allPossibleManifacturers,
            "ProductFullWithXml_popup_modal_content",
            "ProductFullHtmlBased_popup_modal_content",
            "ProductChanges_popup_modal_content",
            "ProductImages_popup_modal_content",
            "ProductFirstImage_popup_modal_content");
    }

    public IActionResult OnPostAddNewImageToProduct(int productId, ProductImagePopupUsageEnum productImagePopupUsage, IFormFile fileInfo)
    {
        if (productId <= 0
            || productImagePopupUsage != ProductImagePopupUsageEnum.DisplayData) return BadRequest();

        string? extensionFronContentType = GetImageFileExtensionFromContentType(fileInfo.ContentType);

        if (extensionFronContentType is null) return BadRequest();

        AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromFileExtension(extensionFronContentType);

        if (allowedImageFileType is null) return BadRequest();

        ProductDisplayData? productDataToAddImageTo = _productTableDataService.GetProductById(productId);

        if (productDataToAddImageTo is null) return BadRequest();

        using MemoryStream stream = new();

        fileInfo.CopyTo(stream);

        byte[] imageBytes = stream.ToArray();

        string contentType = fileInfo.ContentType.ToLower();

        string productHtml = _productHtmlService.GetHtmlFromProduct(MapToProduct(productDataToAddImageTo));

        ProductImage productImage = new()
        {
            ProductId = productId,
            ImageData = imageBytes,
            ImageContentType = contentType,
            HtmlData = productHtml,
        };

        ProductImageFileNameInfo productImageFileNameInfo = new()
        {
            ProductId = productId,
            Active = false,
            DisplayOrder = (productDataToAddImageTo.ImagesAndImageFileInfos?.Count ?? 0) + 1,
        };

        productImageFileNameInfo.FileName = GetTemporaryFileNameFromFileNameInfoAndContentType(productImageFileNameInfo, contentType);

        productDataToAddImageTo.ImagesAndImageFileInfos ??= new();
        productDataToAddImageTo.ImagesAndImageFileInfos.Add(new(productImage, productImageFileNameInfo));

        return OnGetGetPartialViewImagesForProduct(productId, productImagePopupUsage);
    }

    public IActionResult OnPostTriggerChangeCheck()
    {
        _localChangesCheckingImmediateExecutionService.ExecuteImmediately();

        return new OkResult();
    }

    public async Task<IActionResult> OnPutSaveProductWithoutSavingImagesInDBAsync(int productId, [FromBody] ProductDisplayData productDisplayData)
    {
        Product? oldProductData = _productService.GetProductFull(productId);

        if (oldProductData is null
            || productDisplayData is null
            || productDisplayData.ProductNewStatus == ProductNewStatusEnum.New
            || productDisplayData.ProductXmlStatus == ProductXmlStatusEnum.NotReady) return BadRequest();

        ProductDisplayData? localProductDisplayData = _productTableDataService.GetProductById(productId);

        if (localProductDisplayData is null) return BadRequest();

        productDisplayData.ImagesAndImageFileInfos ??= localProductDisplayData.ImagesAndImageFileInfos?.ToList()
            ?? GetImageRelationsFromImagesAndImageFileInfos(
                oldProductData.Images?.ToList(),
                oldProductData.ImageFileNames?.ToList());

        productDisplayData.Properties ??= localProductDisplayData.Properties?.ToList() ?? oldProductData.Properties
            .Select(property => MapToProductPropertyDisplayData(property))
            .ToList();

        List<ImageFileAndFileNameInfoUpsertRequest> imageFileUpsertRequests = new();

        foreach (ImageAndImageFileNameRelation imageAndImageFileNameRelation in productDisplayData.ImagesAndImageFileInfos)
        {
            ImageFileAndFileNameInfoUpsertRequest? imageFileUpsertRequest
                = GetFileUpsertRequest(imageAndImageFileNameRelation, true, true);

            if (imageFileUpsertRequest is null) return BadRequest();

            imageFileUpsertRequests.Add(imageFileUpsertRequest);
        }

        Product productToUpdate = MapToProduct(productDisplayData);

        ProductUpdateRequest productUpdateRequest = MapToUpdateRequestWithoutImagesAndProps(productToUpdate);

        ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDBRequest
            = MapToProductUpdateRequestWithoutImagesInDB(productDisplayData, imageFileUpsertRequests);

        OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> updateProductWithoutImagesInDBResult
            = await _productService.UpdateProductAndUpdateImagesOnlyInDirectoryAsync(productUpdateWithoutImagesInDBRequest);

        IStatusCodeActionResult saveProductResult = updateProductWithoutImagesInDBResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500),
            directoryNotFoundResult => StatusCode(500),
            fileDoesntExistResult => NotFound(fileDoesntExistResult));

        return saveProductResult;
    }

    public async Task<IActionResult> OnPutSaveProductAsync(int productId, [FromBody] ProductDisplayData productDisplayData)
    {
        if (productId <= 0) return BadRequest();

        Product? oldProductData = _productService.GetProductFull(productId);

        if (oldProductData is null
            || productDisplayData is null) return BadRequest();

        productDisplayData = GetMostRecentProductDisplayData(productId, productDisplayData, oldProductData);

        OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addNewImagesResult
            = await AddImagesFromFilesToProductDataAsync(productDisplayData);

        ProductFullUpdateRequest productFullUpdateRequest = GetProductFullUpdateRequest(productDisplayData);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productFullUpdateResult
            = await _productService.UpdateProductFullAsync(productFullUpdateRequest);

        IStatusCodeActionResult productFullUpdateActionResult = productFullUpdateResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (productFullUpdateActionResult.StatusCode != 200)
        {
            return productFullUpdateActionResult;
        }

        if (productDisplayData.ProductWorkStatusesId is null
            || productDisplayData.ProductWorkStatusesId <= 0)
        {
            Product? updatedProduct = _productService.GetProductFull(productId);

            if (updatedProduct is null)
            {
                return StatusCode(500);
            }

            List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringOriginData
                = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(updatedProduct);

            return Partial("ProductPopups/_ProductChangesPopupPartial", new ProductChangesPopupPartialModel(updatedProduct, searchStringOriginData));
        }

        ProductWorkStatusesUpdateByProductIdRequest productWorkStatusesUpdateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = productDisplayData.ProductNewStatus ?? ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = productDisplayData.ProductXmlStatus ?? ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = productDisplayData.ReadyForImageInsert ?? false,
        };

        OneOf<bool, ValidationResult> productWorkStatusesUpdateResult = _productWorkStatusesService.UpdateByProductId(productWorkStatusesUpdateRequest);

        return productWorkStatusesUpdateResult.Match(
            success =>
            {
                if (!success)
                {
                    return NotFound();
                }

                Product? updatedProduct = _productService.GetProductFull(productId);

                if (updatedProduct is null)
                {
                    return StatusCode(500);
                }

                List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringOriginData
                    = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(updatedProduct);

                return Partial("ProductPopups/_ProductChangesPopupPartial", new ProductChangesPopupPartialModel(updatedProduct, searchStringOriginData));
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult));
    }

    private OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> AddImagesFromFilesToProductData(ProductDisplayData productDisplayData)
    {
        if (productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return false;

        for (int i = 0; i < productDisplayData.ImagesAndImageFileInfos.Count; i++)
        {
            ImageAndImageFileNameRelation imageAndImageFileNameRelation = productDisplayData.ImagesAndImageFileInfos[i];

            ProductImage? image = imageAndImageFileNameRelation.ProductImage;
            ProductImageFileNameInfo? fileNameInfo = imageAndImageFileNameRelation.ProductImageFileNameInfo;

            if (image is not null
                || fileNameInfo is null
                || string.IsNullOrWhiteSpace(fileNameInfo.FileName)) continue;

            OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> imageReadResult
                = _productImageFileManagementService.GetImage(fileNameInfo.FileName);

            OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> result = imageReadResult.Match<OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult>>(
                imageData =>
                {
                    ProductImage productImageFromData = new()
                    {
                        Id = -1,
                        ProductId = productDisplayData.Id,
                        ImageData = imageData,
                        ImageContentType = $"image/{GetFileExtensionWithoutDot(fileNameInfo.FileName)}",
                    };

                    productDisplayData.ImagesAndImageFileInfos[i] = new(productImageFromData, fileNameInfo);

                    return true;
                },
                fileDoesntExistResult => fileDoesntExistResult,
                notSupportedFileTypeResult => notSupportedFileTypeResult);

            if (!result.IsT0 || !result.AsT0) return result;
        }

        return true;
    }

    private async Task<OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult>> AddImagesFromFilesToProductDataAsync(ProductDisplayData productDisplayData)
    {
        if (productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return false;

        for (int i = 0; i < productDisplayData.ImagesAndImageFileInfos.Count; i++)
        {
            ImageAndImageFileNameRelation imageAndImageFileNameRelation = productDisplayData.ImagesAndImageFileInfos[i];

            ProductImage? image = imageAndImageFileNameRelation.ProductImage;
            ProductImageFileNameInfo? fileNameInfo = imageAndImageFileNameRelation.ProductImageFileNameInfo;

            if (image is not null
                || fileNameInfo is null
                || string.IsNullOrWhiteSpace(fileNameInfo.FileName)) continue;

            OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> imageReadResult
                = await _productImageFileManagementService.GetImageAsync(fileNameInfo.FileName);

            OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> result = imageReadResult.Match<OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult>>(
                imageData =>
                {
                    ProductImage productImageFromData = new()
                    {
                        Id = -1,
                        ProductId = productDisplayData.Id,
                        ImageData = imageData,
                        ImageContentType = $"image/{GetFileExtensionWithoutDot(fileNameInfo.FileName)}",
                    };

                    productDisplayData.ImagesAndImageFileInfos[i] = new(productImageFromData, fileNameInfo);

                    return true;
                },
                fileDoesntExistResult => fileDoesntExistResult,
                notSupportedFileTypeResult => notSupportedFileTypeResult);

            if (!result.IsT0 || !result.AsT0) return result;
        }

        return true;
    }

    public IActionResult OnPutUpdateProductNewStatus(int productId, ProductNewStatusEnum productNewStatus, string htmlElementId, int tableIndex)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData == null) return BadRequest();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> addOrUpdateProductWorkStatusesResult
            = AddOrUpdateProductWorkStatuses(productDisplayData, newProductNewStatus: productNewStatus);

        int productWorkStatusesId = -1;

        IStatusCodeActionResult addOrUpdateProductWorkStatusesActionResult = addOrUpdateProductWorkStatusesResult.Match(
            id =>
            {
                productWorkStatusesId = id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (addOrUpdateProductWorkStatusesActionResult.StatusCode != 200) return addOrUpdateProductWorkStatusesActionResult;

        productDisplayData.ProductWorkStatusesId = productWorkStatusesId;
        productDisplayData.ProductNewStatus = productNewStatus;
        productDisplayData.ProductXmlStatus = productWorkStatuses?.ProductXmlStatus ?? ProductXmlStatusEnum.NotReady;
        productDisplayData.ReadyForImageInsert = productWorkStatuses?.ReadyForImageInsert ?? false;

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayData);
    }

    public IActionResult OnPutUpdateProductXmlStatus(int productId, ProductXmlStatusEnum productXmlStatus, string htmlElementId, int tableIndex)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData == null) return BadRequest();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> addOrUpdateProductWorkStatusesResult
            = AddOrUpdateProductWorkStatuses(productDisplayData, newProductXmlStatus: productXmlStatus);

        int productWorkStatusesId = -1;

        IStatusCodeActionResult addOrUpdateProductWorkStatusesActionResult = addOrUpdateProductWorkStatusesResult.Match(
            id =>
            {
                productWorkStatusesId = id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (addOrUpdateProductWorkStatusesActionResult.StatusCode != 200) return addOrUpdateProductWorkStatusesActionResult;

        productDisplayData.ProductWorkStatusesId = productWorkStatusesId;
        productDisplayData.ProductNewStatus = productWorkStatuses?.ProductNewStatus ?? ProductNewStatusEnum.New;
        productDisplayData.ProductXmlStatus = productXmlStatus;
        productDisplayData.ReadyForImageInsert = productWorkStatuses?.ReadyForImageInsert ?? false;

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayData);
    }

    public IActionResult OnPutToggleReadyForImageInsertStatus(int productId, string htmlElementId, int tableIndex)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null) return BadRequest();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        bool readyForImageInsert = !productDisplayData.ReadyForImageInsert ?? !productWorkStatuses?.ReadyForImageInsert ?? true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> addOrUpdateProductWorkStatusesResult
            = AddOrUpdateProductWorkStatuses(productDisplayData, newReadyForImageInsertStatus: readyForImageInsert);

        int productWorkStatusesId = -1;

        IStatusCodeActionResult addOrUpdateProductWorkStatusesActionResult = addOrUpdateProductWorkStatusesResult.Match(
            id =>
            {
                productWorkStatusesId = id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (addOrUpdateProductWorkStatusesActionResult.StatusCode != 200) return addOrUpdateProductWorkStatusesActionResult;

        productDisplayData.ProductWorkStatusesId = productWorkStatusesId;
        productDisplayData.ProductNewStatus = productWorkStatuses?.ProductNewStatus ?? ProductNewStatusEnum.New;
        productDisplayData.ProductXmlStatus = productWorkStatuses?.ProductXmlStatus ?? ProductXmlStatusEnum.WorkInProgress;
        productDisplayData.ReadyForImageInsert = readyForImageInsert;

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayData);
    }

    private OneOf<int, ValidationResult, UnexpectedFailureResult> AddOrUpdateProductWorkStatuses(
        ProductDisplayData productDisplayData,
        ProductNewStatusEnum? newProductNewStatus = null,
        ProductXmlStatusEnum? newProductXmlStatus = null,
        bool? newReadyForImageInsertStatus = null)
    {
        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productDisplayData.Id);

        if (productWorkStatuses is null)
        {
            OneOf<int, ValidationResult, UnexpectedFailureResult> productStatusesInsertResult = _productWorkStatusesService.InsertIfItDoesntExist(new()
            {
                ProductId = productDisplayData.Id,
                ProductNewStatus = newProductNewStatus ?? ProductNewStatusEnum.New,
                ProductXmlStatus = newProductXmlStatus ?? ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = newReadyForImageInsertStatus ?? false,
            });

            return productStatusesInsertResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
                id => id,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<bool, ValidationResult> productStatusesUpdateResult = _productWorkStatusesService.UpdateByProductId(new()
        {
            ProductId = productDisplayData.Id,
            ProductNewStatus = newProductNewStatus ?? productWorkStatuses.ProductNewStatus,
            ProductXmlStatus = newProductXmlStatus ?? productWorkStatuses.ProductXmlStatus,
            ReadyForImageInsert = newReadyForImageInsertStatus ?? productWorkStatuses.ReadyForImageInsert,
        });

        return productStatusesUpdateResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => productWorkStatuses.Id,
            validationResult => validationResult);
    }

    public IActionResult OnPutUpdateImageDisplayOrder(int productId, int oldDisplayOrder, int newDisplayOrder, ProductImagePopupUsageEnum productImagePopupUsage)
    {
        if (productId <= 0
            || oldDisplayOrder <= 0
            || newDisplayOrder <= 0
            || productImagePopupUsage != ProductImagePopupUsageEnum.DisplayData) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null
            || productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return NotFound();

        foreach (ImageAndImageFileNameRelation imageAndFileNameInfoRelation in productDisplayData.ImagesAndImageFileInfos)
        {
            ProductImageFileNameInfo? productImageFileNameInfo = imageAndFileNameInfoRelation.ProductImageFileNameInfo;

            if (productImageFileNameInfo?.DisplayOrder is null) continue;

            productImageFileNameInfo.DisplayOrder = ChangeDisplayOrderBasedOnLastChange(
                oldDisplayOrder, newDisplayOrder, productImageFileNameInfo.DisplayOrder.Value);
        }

        return Partial("ProductPopups/_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(
            ProductImagePopupUsageEnum.DisplayData,
            productDisplayData,
            _productImageService,
            _productImageFileNameInfoService,
            _productImageFileManagementService,
            "ProductImages_popup_modal_content",
            "topNotificationBox"));
    }

    private static int? ChangeDisplayOrderBasedOnLastChange(int oldDisplayOrder, int newDisplayOrder, int displayOrder)
    {
        if (oldDisplayOrder <= 0
            || newDisplayOrder <= 0) return null;

        if (displayOrder == oldDisplayOrder)
        {
            displayOrder = newDisplayOrder;
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

    public IActionResult OnPutUpdateImageActiveStatus(int productId, int displayOrder, bool active)
    {
        if (productId <= 0
            || displayOrder <= 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null
            || productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return BadRequest();

        ProductImageFileNameInfo? item = productDisplayData.ImagesAndImageFileInfos.Find(
            x => x.ProductImageFileNameInfo?.DisplayOrder == displayOrder)?
            .ProductImageFileNameInfo;

        if (item is null) return BadRequest();

        item.Active = active;

        return new OkResult();
    }

    public IActionResult OnPutAlterImageHtmlData(int productId, int[] imageIndexes, string htmlData)
    {
        if (productId <= 0
            || imageIndexes.Length <= 0)
        {
            return BadRequest();
        }

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null
            || productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return BadRequest();


        foreach (int imageIndex in imageIndexes)
        {
            if (imageIndex < 0
                || imageIndex >= productDisplayData.ImagesAndImageFileInfos.Count)
            {
                return BadRequest();
            }
        }

        foreach (int imageIndex in imageIndexes)
        {
            ImageAndImageFileNameRelation imageAndImageFileNameRelation = productDisplayData.ImagesAndImageFileInfos[imageIndex];

            if (imageAndImageFileNameRelation.ProductImage is null) continue;

            imageAndImageFileNameRelation.ProductImage.HtmlData = htmlData;
        }

        return new OkResult();
    }

    public IActionResult OnPutAlterAllImagesHtmlData(
        int productId, ProductImagePopupUsageEnum popupUsage, [FromBody] string htmlData)
    {
        if (productId <= 0)
        {
            return BadRequest();
        }

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null
            || productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0)
        {
            return BadRequest();
        }

        foreach (ImageAndImageFileNameRelation imageAndImageFileNameRelation in productDisplayData.ImagesAndImageFileInfos)
        {
            if (imageAndImageFileNameRelation.ProductImage is null) continue;

            imageAndImageFileNameRelation.ProductImage.HtmlData = htmlData;
        }

        return OnGetGetPartialViewImagesForProduct(productId, popupUsage);
    }

    public IActionResult OnDeleteDeleteProduct(int productId)
    {
        if (productId <= 0)
        {
            ModelState.AddModelError(nameof(productId), "Id must be greater than 0.");

            return BadRequest(ModelState);
        }

        ProductDisplayData? productDataAtIndex = _productTableDataService.ProductData.FirstOrDefault(x => x.Id == productId);

        if (productDataAtIndex == null)
        {
            return BadRequest();
        }

        bool isDeleteOfProductSuccessful = _productService.Delete(productDataAtIndex.Id);

        if (!isDeleteOfProductSuccessful)
        {
            return BadRequest();
        }

        bool isRemoveProductFromServiceSuccessful = _productTableDataService.RemoveProductById(productId);

        if (!isRemoveProductFromServiceSuccessful)
        {
            return StatusCode(500);
        }

        return new OkResult();
    }

    public IActionResult OnDeleteDeleteImageFromProduct(int productId, int imageIndex, ProductImagePopupUsageEnum productImagePopupUsage)
    {
        if (productId <= 0
            || imageIndex < 0
            || productImagePopupUsage == ProductImagePopupUsageEnum.ImagesInDatabase) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData?.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return BadRequest();

        if (imageIndex >= productDisplayData.ImagesAndImageFileInfos.Count) return BadRequest();

        ImageAndImageFileNameRelation imageAndFileNameRelation = productDisplayData.ImagesAndImageFileInfos[imageIndex];

        int? displayOrderOfImage = imageAndFileNameRelation.ProductImageFileNameInfo?.DisplayOrder;

        productDisplayData.ImagesAndImageFileInfos.RemoveAt(imageIndex);

        if (displayOrderOfImage is null) return new OkResult();

        foreach (ImageAndImageFileNameRelation relation in productDisplayData.ImagesAndImageFileInfos)
        {
            if (relation.ProductImageFileNameInfo is null
                || relation.ProductImageFileNameInfo.DisplayOrder <= displayOrderOfImage) continue;

            relation.ProductImageFileNameInfo.DisplayOrder--;
        }

        if (productImagePopupUsage == ProductImagePopupUsageEnum.ImagesInFiles
            && imageAndFileNameRelation.ProductImageFileNameInfo?.FileName is not null)
        {
            bool isProductImageFileNameDeleted = _productImageFileNameInfoService.DeleteByProductIdAndImageNumber(
                productId, imageAndFileNameRelation.ProductImageFileNameInfo.ImageNumber);

            if (!isProductImageFileNameDeleted) return BadRequest();

            _productImageFileManagementService.DeleteFile(imageAndFileNameRelation.ProductImageFileNameInfo.FileName);

            return new OkResult();
        }

        return new OkResult();
    }
}