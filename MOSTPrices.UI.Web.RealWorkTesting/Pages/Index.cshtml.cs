using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneOf.Types;
using MOSTComputers.UI.Web.RealWorkTesting.Utils;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using System.Transactions;
using MOSTComputers.Services.ProductImageFileManagement.Models;

using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductToDisplayDataMappingUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductMappingUtils;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductImageFileManagement.Utils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages;

[Authorize]
public class IndexModel : PageModel
{
    #region Temporary test data

    private const bool _useOtherProductTestDataWhenAdding = true;
    private const int _testDataOtherProductId = 68836;

    #endregion Temporary test data

    public IndexModel(IProductService productService,
        ICategoryService categoryService,
        IManifacturerService manifacturerService,
        IProductTableDataService productTableDataService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductImageSaveService productImageSaveService,
        IProductWorkStatusesService productWorkStatusesService,
        ISearchStringOriginService searchStringOriginService,
        ILocalChangesCheckingImmediateExecutionService localChangesCheckingImmediateExecutionService,
        IProductImageFileManagementService productImageFileManagementService,
        ITransactionExecuteService transactionExecuteService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _manifacturerService = manifacturerService;
        _productTableDataService = productTableDataService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productImageSaveService = productImageSaveService;
        _productWorkStatusesService = productWorkStatusesService;
        _searchStringOriginService = searchStringOriginService;
        _localChangesCheckingImmediateExecutionService = localChangesCheckingImmediateExecutionService;
        _productImageFileManagementService = productImageFileManagementService;
        _transactionExecuteService = transactionExecuteService;

        Products = productTableDataService.PopulateIfEmptyAndGet(PopulateProductDisplayData);

        ProductsToDisplay = Products;

        Product? productWithHighestId = _productService.GetProductWithHighestId();

        if (productWithHighestId is not null)
        {
            SingleEditedProduct = MapToProductDisplayData(productWithHighestId);
        }
    }

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IManifacturerService _manifacturerService;
    private readonly IProductTableDataService _productTableDataService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageSaveService _productImageSaveService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly ILocalChangesCheckingImmediateExecutionService _localChangesCheckingImmediateExecutionService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public IReadOnlyList<ProductDisplayData> Products { get; set; }
    public IReadOnlyList<ProductDisplayData> ProductsToDisplay { get; set; }
    public ProductDisplayData? SingleEditedProduct { get; set; }

    private List<ProductDisplayData> PopulateProductDisplayData()
    {
        List<ProductDisplayData> output = new();

        IEnumerable<Product> firstProducts = _productService.GetFirstItemsBetweenStartAndEnd(new() { Start = 0, Length = 12 });

        foreach (Product product in firstProducts)
        {
            product.Images = _productImageService.GetAllInProduct((uint)product.Id)
                .ToList();

            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct((uint)product.Id)
                .ToList();

            product.Properties = _productPropertyService.GetAllInProduct((uint)product.Id)
                .ToList();

            ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(product.Id);

            ProductDisplayData productDisplayData = MapToProductDisplayData(product, productWorkStatuses);

            OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addImagesFromFilesResult
                = AddImagesFromFilesToProductData(productDisplayData);

            output.Add(productDisplayData);
        }

        return output;
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

        return Partial("ProductPopups/_ProductFirstImageDisplayPopupPartial", new ProductFirstImageDisplayPopupPartialModel(productDataToDisplay));
    }

    public IActionResult OnGetGetPartialViewImagesForProduct(int productId)
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

        return Partial("ProductPopups/_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(productDataToDisplay));
    }

    public IActionResult OnGetGetProductChangesPopupPartialViewForProduct(int productId, bool getEvenIfProductDoesntExist = false)
    {
        if (productId < 0)
        {
            return BadRequest("Id cannot be negative");
        }

        Product? productToDisplay = _productService.GetByIdWithImages((uint)productId);

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

    public IActionResult OnPostAddNewProduct(string htmlElementId, int tableIndex)
    {
        ProductCreateRequest productCreateRequest = new()
        {
        };

        if (_useOtherProductTestDataWhenAdding)
        {
            Product? otherProductData = _productService.GetByIdWithProps(_testDataOtherProductId);

            if (otherProductData != null)
            {
                productCreateRequest = MapProductToCreateRequest(otherProductData);
            }
        }

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(productCreateRequest);

        int newProductId = -1;

        IStatusCodeActionResult productInsertActionResult = productInsertResult.Match(
            id =>
            {
                newProductId = (int)id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (productInsertActionResult.StatusCode != 200) return productInsertActionResult;

        Product? productToDisplay = GetProductBasedOnTestData(newProductId);

        if (productToDisplay is null) return StatusCode(500);

        ProductWorkStatusesCreateRequest productWorkStatusesCreateRequest = new()
        {
            ProductId = newProductId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> productWorkStatusesInsertResult
            = _productWorkStatusesService.InsertIfItDoesntExist(productWorkStatusesCreateRequest);

        int productWorkStatusesId = -1;

        IStatusCodeActionResult productWorkStatusesInsertActionResult = productWorkStatusesInsertResult.Match(
            id =>
            {
                productWorkStatusesId = id;

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (productWorkStatusesInsertActionResult.StatusCode != 200)
        {
            return productWorkStatusesInsertActionResult;
        }

        ProductWorkStatuses productWorkStatuses = new()
        {
            Id = productWorkStatusesId,
            ProductId = newProductId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        ProductDisplayData productDisplayData = MapToProductDisplayData(productToDisplay, productWorkStatuses);

        _productTableDataService.AddProductData(productDisplayData, true);

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productToDisplay);
    }

    public async Task<IActionResult> OnPostAddNewProductWithExistingProductDataAsync(
        int productToCopyFromId, int highestId, string htmlElementId, int tableIndex, [FromBody] ProductDisplayData productDisplayData)
    {
        if (productToCopyFromId <= 0) return BadRequest();

        ProductDisplayData? productDisplayDataOfProduct = GetMostRecentProductDisplayData(productToCopyFromId, productDisplayData);

        if (productDisplayDataOfProduct is null) return BadRequest();

        int newProductId = highestId + 1;

        productDisplayDataOfProduct.Id = newProductId;

        if (productDisplayDataOfProduct.ImagesAndImageFileInfos is not null)
        {
            for (int i = 0; i < productDisplayDataOfProduct.ImagesAndImageFileInfos.Count; i++)
            {
                Tuple<ProductImage?, ProductImageFileNameInfo?> tuple = productDisplayDataOfProduct.ImagesAndImageFileInfos[i];

                if (tuple.Item1?.ImageFileExtension is null
                    || tuple.Item1.ImageData is null
                    || tuple.Item2 is null
                    || tuple.Item2.ProductId <= 0) continue;

                tuple.Item2.ProductId = newProductId;

                ProductImage newProductImageFromExistingData = new()
                {
                    Id = 0,
                    ProductId = newProductId,
                    ImageData = tuple.Item1.ImageData,
                    ImageFileExtension = tuple.Item1.ImageFileExtension,
                };

                if (tuple.Item2 is null)
                {
                    ProductImageFileNameInfo newFileNameInfo = new()
                    {
                        ProductId = newProductId,
                        Active = false,
                        DisplayOrder = GetLowestUnpopulatedDisplayOrder(productDisplayDataOfProduct),
                    };

                    newFileNameInfo.FileName = GetTemporaryIdFromFileNameInfoAndContentType(newFileNameInfo, tuple.Item1.ImageFileExtension);

                    productDisplayDataOfProduct.ImagesAndImageFileInfos[i] = new(newProductImageFromExistingData, newFileNameInfo);

                    continue;
                }

                tuple.Item2.FileName = GetTemporaryIdFromFileNameInfoAndContentType(tuple.Item2, tuple.Item1.ImageFileExtension);

                productDisplayDataOfProduct.ImagesAndImageFileInfos[i] = new(newProductImageFromExistingData, tuple.Item2);
            }
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> insertFullWithImagesOnlyInDirectoryResult
            = await InsertProductFullWithImagesOnlyInDirectoryAsync(productDisplayDataOfProduct);

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

        Product? newProduct = GetProductFull(newProductId)!;

        ProductDisplayData? productDisplayDataInEditor = _productTableDataService.ProductData.FirstOrDefault(x => x.Id == newProductId);

        if (productDisplayDataInEditor == null)
        {
            productDisplayDataInEditor = MapToProductDisplayData(newProduct!);

            _productTableDataService.AddProductData(productDisplayDataInEditor, true);
        }

        productDisplayDataInEditor.ImagesAndImageFileInfos = productDisplayDataOfProduct.ImagesAndImageFileInfos;

        productDisplayDataInEditor.ProductNewStatus = ProductNewStatusEnum.New;
        productDisplayDataInEditor.ProductXmlStatus = ProductXmlStatusEnum.NotReady;
        productDisplayDataInEditor.ReadyForImageInsert = false;

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, productDisplayDataInEditor);
    }

    private ProductDisplayData? GetMostRecentProductDisplayData(int productToCopyFromId, ProductDisplayData? productDisplayData)
    {
        Product? product = GetProductFull(productToCopyFromId);

        if (product is null) return null;

        ProductDisplayData? productDisplayDataOfProduct;

        ProductDisplayData savedProductDisplayData = _productTableDataService.GetProductById(productToCopyFromId)
            ?? MapToProductDisplayData(product);

        productDisplayDataOfProduct = (productDisplayData is not null) ? productDisplayData : savedProductDisplayData;

        productDisplayDataOfProduct.ImagesAndImageFileInfos = savedProductDisplayData.ImagesAndImageFileInfos?.ToList();

        productDisplayDataOfProduct.Properties = savedProductDisplayData.Properties?.ToList();

        return CloneProductDisplayData(productDisplayDataOfProduct);
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertProductFullWithImagesOnlyInDirectoryAsync(ProductDisplayData productDataToAdd)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertProductFullWithImagesOnlyInDirectoryInternalAsync(productDataToAdd),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false,
                fileDoesntExistResult => false));
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertProductFullWithImagesOnlyInDirectoryInternalAsync(ProductDisplayData productDataToAdd)
    {
        List<Tuple<ProductImage?, ProductImageFileNameInfo?>>? productImages = productDataToAdd.ImagesAndImageFileInfos?.ToList();

        productDataToAdd.ImagesAndImageFileInfos = null;

        Product productToAdd = MapToProduct(productDataToAdd);

        ProductCreateRequest productCreateRequest = MapProductToCreateRequest(productToAdd);

        productDataToAdd.ImagesAndImageFileInfos = productImages;

        productDataToAdd = CloneProductDisplayData(productDataToAdd);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(productCreateRequest);

        int productId = -1;

        bool isProductInsertSuccessful = productInsertResult.Match(
            id =>
            {
                productId = (int)id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isProductInsertSuccessful
            || productImages is null)
        {
            return productInsertResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                id => (int)id,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        productDataToAdd.Id = productId;

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in productImages)
        {
            if (tuple.Item2?.FileName is null
                || tuple.Item1?.ImageData is null) continue;

            string fileExtension = ProductImageFileManagementUtils.GetFileExtensionWithoutDot(tuple.Item2.FileName);

            AllowedImageFileType? allowedImageFileType = ProductImageFileManagementUtils.GetAllowedImageFileTypeFromFileExtension(fileExtension);

            if (allowedImageFileType is null) continue;

            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> imageInsertInDirectoryResult
                = await SaveImageInDirectoryBasedOnDataAsync(productDataToAdd, tuple.Item1, tuple.Item2);

            bool isImageInsertInDirectorySuccessful = imageInsertInDirectoryResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false,
                fileDoesntExistResult => false);

            if (!isImageInsertInDirectorySuccessful)
            {
                return imageInsertInDirectoryResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                    success => productId,
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult,
                    directoryNotFoundResult => directoryNotFoundResult,
                    fileDoesntExistResult => fileDoesntExistResult);
            }
        }

        return productId;
    }

    private static int GetLowestUnpopulatedDisplayOrder(ProductDisplayData productDisplayData)
    {
        if (productDisplayData.ImagesAndImageFileInfos is null) return 1;

        int currentDisplayOrder = 1;

        bool foundCurrentDisplayOrder = false;

        while(foundCurrentDisplayOrder)
        {
            foundCurrentDisplayOrder = false;

            foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in productDisplayData.ImagesAndImageFileInfos)
            {
                if (tuple.Item2?.DisplayOrder == currentDisplayOrder)
                {
                    foundCurrentDisplayOrder = true;

                    break;
                }
            }

            currentDisplayOrder++;
        }

        return currentDisplayOrder;
    }

    public async Task<IActionResult> OnPostGetOnlyProductWithHighestIdAsync(string htmlElementId, int tableIndex)
    {
        Product? product = _productService.GetProductWithHighestId();

        if (product is null) return NotFound();

        product = GetProductFull(product.Id)!;

        SingleEditedProduct = MapToProductDisplayData(product);

        _productTableDataService.AddProductDataIfProductDataWithSameIdDoesntExist(SingleEditedProduct);

        OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addImageFilesResult
            = await AddImagesFromFilesToProductDataAsync(SingleEditedProduct);

        return addImageFilesResult.Match<IStatusCodeActionResult>(
            isSuccessful =>
            {
                return GetPartialTableRowFromProduct(htmlElementId, tableIndex, MapToProduct(SingleEditedProduct));
            },
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            notSupportedFileTypeResult => BadRequest());

    }

    public IActionResult OnPostGetOnlyProductWithId(int productId, string htmlElementId, int tableIndex)
    {
        if (productId < 0) return BadRequest();

        Product? product = _productService.GetByIdWithProps((uint)productId);

        if (product is null) return NotFound();

        ProductDisplayData productDisplayData = MapToProductDisplayData(product);

        _productTableDataService.AddProductDataIfProductDataWithSameIdDoesntExist(productDisplayData);

        return GetPartialTableRowFromProduct(htmlElementId, tableIndex, product);
    }

    private PartialViewResult GetPartialTableRowFromProduct(
        string htmlElementId, int tableIndex, Product productToDisplay)
    {
        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productToDisplay.Id);

        ProductDisplayData productDisplayData = MapToProductDisplayData(productToDisplay, productWorkStatuses);

        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        IEnumerable<SelectListItem> categoryelectListItems = ProductSelectListItemUtils.GetCategorySelectListItems(productToDisplay, allPossibleCategories);
        IEnumerable<SelectListItem> manifacturerSelectListItems = ProductSelectListItemUtils.GetManifacturerSelectListItems(productToDisplay, allPossibleManifacturers);
        IEnumerable<SelectListItem> statusSelectListItems = ProductSelectListItemUtils.GetStatusSelectListItems(productToDisplay);
        IEnumerable<SelectListItem> currencySelectListItems = ProductSelectListItemUtils.GetCurrencySelectListItems(productToDisplay);
        IEnumerable<SelectListItem> productNewStatusSelectListItems = ProductSelectListItemUtils.GetProductNewStatusSelectListItems(productDisplayData);
        IEnumerable<SelectListItem> productXmlStatusSelectListItems = ProductSelectListItemUtils.GetProductXmlStatusSelectListItems(productDisplayData);

        IndexProductTableRowPartialModel productPartialModel = new(
            productDisplayData,
            htmlElementId,
            tableIndex,
            categoryelectListItems,
            manifacturerSelectListItems,
            statusSelectListItems,
            currencySelectListItems,
            productNewStatusSelectListItems,
            productXmlStatusSelectListItems);

        return base.Partial("_IndexProductTableRowPartial", productPartialModel);
    }

    private PartialViewResult GetPartialTableRowFromProduct(
        string htmlElementId, int tableIndex, ProductDisplayData productDisplayData)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        IEnumerable<SelectListItem> categoryelectListItems = ProductSelectListItemUtils.GetCategorySelectListItems(productDisplayData, allPossibleCategories);
        IEnumerable<SelectListItem> manifacturerSelectListItems = ProductSelectListItemUtils.GetManifacturerSelectListItems(productDisplayData, allPossibleManifacturers);
        IEnumerable<SelectListItem> statusSelectListItems = ProductSelectListItemUtils.GetStatusSelectListItems(productDisplayData);
        IEnumerable<SelectListItem> currencySelectListItems = ProductSelectListItemUtils.GetCurrencySelectListItems(productDisplayData);
        IEnumerable<SelectListItem> productNewStatusSelectListItems = ProductSelectListItemUtils.GetProductNewStatusSelectListItems(productDisplayData);
        IEnumerable<SelectListItem> productXmlStatusSelectListItems = ProductSelectListItemUtils.GetProductXmlStatusSelectListItems(productDisplayData);

        IndexProductTableRowPartialModel productPartialModel = new(
            productDisplayData,
            htmlElementId,
            tableIndex,
            categoryelectListItems,
            manifacturerSelectListItems,
            statusSelectListItems,
            currencySelectListItems,
            productNewStatusSelectListItems,
            productXmlStatusSelectListItems);

        return base.Partial("_IndexProductTableRowPartial", productPartialModel);
    }

    public IActionResult OnPostAddNewImageToProduct(int productId, IFormFile fileInfo)
    {
        if (productId <= 0) return BadRequest();

        ProductDisplayData? productDataToAddImageTo = _productTableDataService.ProductData.FirstOrDefault(
            product => product.Id == productId);

        if (productDataToAddImageTo is null) return BadRequest();

        using MemoryStream stream = new();

        fileInfo.CopyTo(stream);

        byte[] imageBytes = stream.ToArray();

        string contentType = fileInfo.ContentType;

        ProductImage productImage = new()
        {
            ProductId = productId,
            ImageData = imageBytes,
            ImageFileExtension = contentType.ToLower(),
        };

        ProductImageFileNameInfo productImageFileNameInfo = new()
        {
            ProductId = productId,
            Active = false,
            DisplayOrder = (productDataToAddImageTo.ImagesAndImageFileInfos?.Count ?? 0) + 1
        };

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null) return BadRequest();

        productDisplayData.ImagesAndImageFileInfos ??= new();
        productDisplayData.ImagesAndImageFileInfos.Add(new(productImage, productImageFileNameInfo));

        return OnGetGetPartialViewImagesForProduct(productId);
    }

    private Product? GetProductBasedOnTestData(int newProductId)
    {
        Product? productToDisplay;

#pragma warning disable CS0162 // Unreachable code detected
        // Reason: _useOtherProductTestDataWhenAdding will change for testing purposes
        if (!_useOtherProductTestDataWhenAdding)
        {
            productToDisplay = new() { Id = newProductId };
#pragma warning restore CS0162 // Unreachable code detected
        }
        else
        {
            productToDisplay = _productService.GetByIdWithProps((uint)newProductId);
        }

        return productToDisplay;
    }

    public IActionResult OnPostTriggerChangeCheck()
    {
        _localChangesCheckingImmediateExecutionService.ExecuteImmediately();

        return new OkResult();
    }

    public async Task<IActionResult> OnPutSaveProductWithoutSavingImagesInDBAsync(int productId, [FromBody] ProductDisplayData productDisplayData)
    {
        Product? oldProductData = GetProductFull(productId);

        if (oldProductData is null
            || productDisplayData is null
            || productDisplayData.ProductNewStatus == ProductNewStatusEnum.New
            || productDisplayData.ProductXmlStatus == ProductXmlStatusEnum.NotReady) return BadRequest();

        ProductDisplayData? localProductDisplayData = _productTableDataService.GetProductById(productId);

        if (localProductDisplayData is null) return BadRequest();

        productDisplayData.ImagesAndImageFileInfos ??= localProductDisplayData.ImagesAndImageFileInfos?.ToList()
            ?? GetImageDictionaryFromImagesAndImageFileInfos(
                oldProductData.Images?.ToList(),
                oldProductData.ImageFileNames?.ToList());

        productDisplayData.Properties ??= localProductDisplayData.Properties?.ToList() ?? oldProductData.Properties.ToList();

        Product productToUpdate = MapToProduct(productDisplayData);

        ProductUpdateRequest productUpdateRequest = MapToUpdateRequestWithoutImagesAndProps(productToUpdate);

        IStatusCodeActionResult saveProductResult = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => SaveProductWithoutSavingImagesInDBInternalAsync(productDisplayData, productUpdateRequest, productToUpdate, oldProductData),
            result => result.StatusCode == 200);

        return saveProductResult;
    }

    private async Task<IStatusCodeActionResult> SaveProductWithoutSavingImagesInDBInternalAsync(
        ProductDisplayData productDisplayData, ProductUpdateRequest productUpdateRequest, Product productFull, Product oldProduct)
    {
        IStatusCodeActionResult propertyUpdateActionResult = UpdateProperties(productFull, oldProduct, (uint)oldProduct.Id, productUpdateRequest);

        if (propertyUpdateActionResult.StatusCode != 200) return propertyUpdateActionResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        IStatusCodeActionResult productUpdateActionResult = productUpdateResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));

        if (productUpdateActionResult.StatusCode != 200) return productUpdateActionResult;

        if  (productDisplayData.ImagesAndImageFileInfos is null) return BadRequest();

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in productDisplayData.ImagesAndImageFileInfos)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> saveImageResult
                = await SaveImageInDirectoryBasedOnDataAsync(productDisplayData, tuple.Item1, tuple.Item2);

            IStatusCodeActionResult saveImageActionResult = saveImageResult.Match(
                success => new OkResult(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(500),
                directoryNotFoundResult => StatusCode(500),
                fileDoesntExistResult => BadRequest(fileDoesntExistResult));

            if (saveImageActionResult.StatusCode != 200) return saveImageActionResult;
        }

        return new OkResult();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> SaveImageInDirectoryBasedOnDataAsync(
        ProductDisplayData productDisplayData,
        ProductImage? image,
        ProductImageFileNameInfo? fileNameInfo)
    {
        if (image is null
            && fileNameInfo is null) return new UnexpectedFailureResult();

        if (fileNameInfo is null)
        {
            if (image!.Id > 0
                && image!.ImageData is not null
                && !string.IsNullOrWhiteSpace(image.ImageFileExtension))
            {
                return await HandleImageFileNameNullValidCaseAsync(productDisplayData, image);
            }

            List<ValidationFailure> validationFailures = new();

            if (image!.Id <= 0)
            {
                validationFailures.Add(new(nameof(ProductImage.Id), "Cannot be less than 0"));
            }

            if (image!.ImageData is null)
            {
                validationFailures.Add(new(nameof(ProductImage.ImageData), "Cannot be null"));
            }

            if (string.IsNullOrWhiteSpace(image.ImageFileExtension))
            {
                validationFailures.Add(new(nameof(ProductImage.ImageFileExtension), "Cannot be null, empty, or whitespace"));
            }

            return new ValidationResult(validationFailures);
        }
        else if (image is null)
        {
            if (string.IsNullOrWhiteSpace(fileNameInfo.FileName)) return new UnexpectedFailureResult();

            bool doesFileExist = _productImageFileManagementService.CheckIfImageFileExists(fileNameInfo.FileName);

            return doesFileExist ? new Success() : new FileDoesntExistResult() { FileName = fileNameInfo.FileName };
        }

        return await HandleImageAndFileNameInfoNotNullCaseAsync(image, fileNameInfo);

        async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> HandleImageFileNameNullValidCaseAsync(
            ProductDisplayData productDisplayData, ProductImage image)
        {
            string? imageFileName = GetImageFileNameFromImageData(image.Id, image.ImageFileExtension!);

            if (imageFileName is null) return new UnexpectedFailureResult();

            ServiceProductImageFileNameInfoCreateRequest imageFileNameInfoCreateRequest = new()
            {
                ProductId = productDisplayData.Id,
                FileName = imageFileName,
                Active = false,
                DisplayOrder = GetLowestUnpopulatedDisplayOrder(productDisplayData)
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(imageFileNameInfoCreateRequest);

            bool isimageFileNameInfoInsertSuccessful = imageFileNameInfoInsertResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false);

            if (!isimageFileNameInfoInsertSuccessful)
            {
                return imageFileNameInfoInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                    success => success,
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            string localFileExtension = ProductImageFileManagementUtils.GetFileExtensionWithoutDot(imageFileName);

            AllowedImageFileType? localAllowedImageFileType = ProductImageFileManagementUtils.GetAllowedImageFileTypeFromFileExtension(localFileExtension);

            if (localAllowedImageFileType is null)
            {
                List<ValidationFailure> validationFailuresLocal = new()
                    {
                        new(nameof(ProductImageFileNameInfo.FileName), "Unsupported file type")
                    };

                return new ValidationResult(validationFailuresLocal);
            }

            OneOf<Success, DirectoryNotFoundResult> imageFileCreateResult
                = await _productImageFileManagementService.AddOrUpdateImageAsync(image.Id.ToString(), image.ImageData!, localAllowedImageFileType);

            return imageFileCreateResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                success => success,
                directoryNotFoundResult => directoryNotFoundResult);
        }

        async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> HandleImageAndFileNameInfoNotNullCaseAsync(
            ProductImage image, ProductImageFileNameInfo fileNameInfo)
        {
            List<ValidationFailure> finalValidationFailures = new();

            if (image!.ImageData is null)
            {
                finalValidationFailures.Add(new(nameof(ProductImage.ImageData), "Cannot be null"));
            }

            if (string.IsNullOrWhiteSpace(image.ImageFileExtension))
            {
                finalValidationFailures.Add(new(nameof(ProductImage.ImageFileExtension), "Cannot be null, empty, or whitespace"));
            }

            if (finalValidationFailures.Count > 0)
            {
                return new ValidationResult(finalValidationFailures);
            }

            if (image.Id <= 0)
            {
                fileNameInfo.FileName ??= GetTemporaryIdFromFileNameInfoAndContentType(fileNameInfo, image.ImageFileExtension!);
            }

            if (fileNameInfo.FileName is null) return new UnexpectedFailureResult();

            string fileName = fileNameInfo.FileName;

            string fileExtension = ProductImageFileManagementUtils.GetFileExtensionWithoutDot(fileName);

            AllowedImageFileType? allowedImageFileType = ProductImageFileManagementUtils.GetAllowedImageFileTypeFromFileExtension(fileExtension);

            if (allowedImageFileType is null)
            {
                List<ValidationFailure> validationFailuresLocal = new()
                {
                    new(nameof(ProductImageFileNameInfo.FileName), "Unsupported file type")
                };

                return new ValidationResult(validationFailuresLocal);
            }

            OneOf<int, (int, int), False> idOrTemporaryId = GetImageIdOrTempIdFromImageFileName(fileName);

            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> imageInsertResult
                = await idOrTemporaryId.Match<Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>>(
                async imageId =>
                {
                    return await AddOrUpdateFileNameInfoAndImageFileAsync(
                        productDisplayData, image, fileNameInfo, imageId.ToString(), allowedImageFileType);
                },
                async temporaryIdParts =>
                {
                    string temporaryFileNameToUseWithoutExtension = $"{temporaryIdParts.Item1}-{temporaryIdParts.Item2}";

                    return await AddOrUpdateFileNameInfoAndImageFileAsync(
                        productDisplayData, image, fileNameInfo, temporaryFileNameToUseWithoutExtension, allowedImageFileType);
                },
                async falseResult => await Task.Run(() => new UnexpectedFailureResult()));

            return imageInsertResult;
        }
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        AddOrUpdateFileNameInfoAndImageFileAsync(
        ProductDisplayData productDisplayData,
        ProductImage image,
        ProductImageFileNameInfo fileNameInfo,
        string fileNameToUseWithoutExtension,
        AllowedImageFileType allowedImageFileType)
    {
        fileNameInfo.FileName = $"{fileNameToUseWithoutExtension}.{allowedImageFileType.FileExtension}";

        OneOf<Success, ValidationResult, UnexpectedFailureResult> addOrUpdateFileNameInfoResult
            = AddOrUpdateFileNameInfo(productDisplayData, fileNameInfo);

        bool isimageFileNameInfoInsertSuccessful = addOrUpdateFileNameInfoResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isimageFileNameInfoInsertSuccessful)
        {
            return addOrUpdateFileNameInfoResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                success => success,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<Success, DirectoryNotFoundResult> imageAddOrUpdateResult
            = await _productImageFileManagementService.AddOrUpdateImageAsync(fileNameToUseWithoutExtension, image.ImageData!, allowedImageFileType);

        return imageAddOrUpdateResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
            success => success,
            directoryNotFoundResult => directoryNotFoundResult);
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> AddOrUpdateFileNameInfo(
        ProductDisplayData productDisplayData,
        ProductImageFileNameInfo fileNameInfo)
    {
        ProductImageFileNameInfo? savedImageFileInfo = _productImageFileNameInfoService.GetAllInProduct((uint)productDisplayData.Id)
            .FirstOrDefault(x => x.FileName == fileNameInfo.FileName);

        if (savedImageFileInfo is null)
        {
            ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest = new()
            {
                ProductId = productDisplayData.Id,
                FileName = fileNameInfo.FileName,
                DisplayOrder = fileNameInfo.DisplayOrder ?? GetLowestUnpopulatedDisplayOrder(productDisplayData),
                Active = fileNameInfo.Active,
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

            bool isimageFileNameInfoInsertSuccessful = fileNameInfoInsertResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false);

            return fileNameInfoInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        ServiceProductImageFileNameInfoByImageNumberUpdateRequest imageFileNameInfoUpdateRequest = new()
        {
            ProductId = productDisplayData.Id,
            FileName = fileNameInfo.FileName,
            ImageNumber = savedImageFileInfo.ImageNumber,
            NewDisplayOrder = null,
            Active = fileNameInfo.Active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoUpdateResult
            = _productImageFileNameInfoService.Update(imageFileNameInfoUpdateRequest);

        return fileNameInfoUpdateResult;
    }

    public async Task<IActionResult> OnPutSaveProductAsync(int productId, [FromBody] ProductDisplayData productDisplayData)
    {
        if (productId <= 0) return BadRequest();

        Product? oldProductData = GetProductFull(productId);

        if (oldProductData is null
            || productDisplayData is null) return BadRequest();

        ProductDisplayData? localProductDisplayData = _productTableDataService.GetProductById(productId);

        if (localProductDisplayData is null) return BadRequest();

        productDisplayData.ImagesAndImageFileInfos ??= localProductDisplayData.ImagesAndImageFileInfos?.ToList()
            ?? GetImageDictionaryFromImagesAndImageFileInfos(
                oldProductData.Images?.ToList(),
                oldProductData.ImageFileNames?.ToList());

        productDisplayData.Properties ??= localProductDisplayData.Properties?.ToList() ?? oldProductData.Properties.ToList();

        OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> addNewImagesResult
            = await AddImagesFromFilesToProductDataAsync(productDisplayData);

        Product productToUpdate = MapToProduct(productDisplayData);

        IStatusCodeActionResult productFullUpdateActionResult = SaveProductFull(productToUpdate);

        if (productFullUpdateActionResult.StatusCode != 200
            || productDisplayData.ProductWorkStatusesId is null
            || productDisplayData.ProductWorkStatusesId <= 0)
        {
            return productFullUpdateActionResult;
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

                return new OkResult();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult));
    }

    private static void CorrectImageFileNameData(ProductDisplayData? localProductDisplayData, int newProductId)
    {
        if (localProductDisplayData?.ImagesAndImageFileInfos is null) return;

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in localProductDisplayData.ImagesAndImageFileInfos)
        {
            if (tuple.Item2?.FileName is null
                || tuple.Item1?.ImageFileExtension is null) continue;

            tuple.Item2.ProductId = newProductId;

            OneOf<int, (int, int), False> idOrTempId = GetImageIdOrTempIdFromImageFileName(tuple.Item2.FileName);

            idOrTempId.Switch(
                id => { },
                tempIdParts =>
                {
                    tuple.Item2.FileName = GetTemporaryIdFromFileNameInfoAndContentType(tuple.Item2, tuple.Item1.ImageFileExtension);
                },
                falseResult => { });
        }
    }

    private OneOf<bool, FileDoesntExistResult, NotSupportedFileTypeResult> AddImagesFromFilesToProductData(ProductDisplayData productDisplayData)
    {
        if (productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return false;

        for (int i = 0; i < productDisplayData.ImagesAndImageFileInfos.Count; i++)
        {
            (ProductImage? image, ProductImageFileNameInfo? fileNameInfo) = productDisplayData.ImagesAndImageFileInfos[i];

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
                        ImageFileExtension = $"image/{ProductImageFileManagementUtils.GetFileExtensionWithoutDot(fileNameInfo.FileName)}",
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
            (ProductImage? image, ProductImageFileNameInfo? fileNameInfo) = productDisplayData.ImagesAndImageFileInfos[i];

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
                        ImageFileExtension = $"image/{ProductImageFileManagementUtils.GetFileExtensionWithoutDot(fileNameInfo.FileName)}",
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

    public IActionResult OnPutUpdateProductNewStatus(int productId, ProductNewStatusEnum productNewStatus)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData == null) return BadRequest();

        productDisplayData.ProductNewStatus = productNewStatus;

        return new OkResult();
    }

    public IActionResult OnPutUpdateProductXmlStatus(int productId, ProductXmlStatusEnum productXmlStatus)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData == null) return BadRequest();

        productDisplayData.ProductXmlStatus = productXmlStatus;

        return new OkResult();
    }

    public IActionResult OnPutToggleReadyForImageInsertStatus(int productId)
    {
        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData == null) return BadRequest();

        productDisplayData.ReadyForImageInsert = !productDisplayData.ReadyForImageInsert;

        return new JsonResult(new { updatedReadyForImageInsertValue = productDisplayData.ReadyForImageInsert });
    }

    public IActionResult OnPutUpdateImageDisplayOrder(int productId, int oldDisplayOrder, int newDisplayOrder)
    {
        if (productId <= 0
            || oldDisplayOrder <= 0
            || newDisplayOrder <= 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData is null
            || productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return NotFound();

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in productDisplayData.ImagesAndImageFileInfos)
        {
            if (tuple.Item2?.DisplayOrder is null) continue;

            tuple.Item2.DisplayOrder = ChangeDisplayOrderBasedOnLastChange(oldDisplayOrder, newDisplayOrder, tuple.Item2.DisplayOrder.Value);
        }

        return Partial("ProductPopups/_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(productDisplayData));
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

        ProductImageFileNameInfo? item = productDisplayData.ImagesAndImageFileInfos.Find(x => x.Item2?.DisplayOrder == displayOrder)?.Item2;

        if (item is null) return BadRequest();

        item.Active = active;

        return new OkResult();
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

        bool isDeleteOfProductSuccessful = _productService.Delete((uint)productDataAtIndex.Id);

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

    public IActionResult OnDeleteDeleteImageFromProduct(int productId, int imageIndex)
    {
        if (productId <= 0
            || imageIndex < 0) return BadRequest();

        ProductDisplayData? productDisplayData = _productTableDataService.GetProductById(productId);

        if (productDisplayData?.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return BadRequest();

        if (imageIndex >= productDisplayData.ImagesAndImageFileInfos.Count) return BadRequest();

        Tuple<ProductImage?, ProductImageFileNameInfo?> imageTuple = productDisplayData.ImagesAndImageFileInfos[imageIndex];

        int? displayOrderOfImage = imageTuple.Item2?.DisplayOrder;

        productDisplayData.ImagesAndImageFileInfos.RemoveAt(imageIndex);

        if (displayOrderOfImage is null) return new OkResult();

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in productDisplayData.ImagesAndImageFileInfos)
        {
            if (tuple.Item2 is null
                || tuple.Item2.DisplayOrder <= displayOrderOfImage) continue;

            tuple.Item2.DisplayOrder--;
        }

        return new OkResult();
    }

    public IndexProductTableRowPartialModel GetTableRowModel(Product product, int tableIndex, string htmlElementId)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(product.Id);

        return new IndexProductTableRowPartialModel(
            MapToProductDisplayData(product),
            htmlElementId,
            tableIndex,
            ProductSelectListItemUtils.GetCategorySelectListItems(product, allPossibleCategories),
            ProductSelectListItemUtils.GetManifacturerSelectListItems(product, allPossibleManifacturers),
            ProductSelectListItemUtils.GetStatusSelectListItems(product),
            ProductSelectListItemUtils.GetCurrencySelectListItems(product),
            ProductSelectListItemUtils.GetProductNewStatusSelectListItems(productWorkStatuses),
            ProductSelectListItemUtils.GetProductXmlStatusSelectListItems(productWorkStatuses));
    }

    public IndexProductTableRowPartialModel GetTableRowModel(ProductDisplayData productData, int tableIndex, string htmlElementId)
    {
        IEnumerable<Category> allPossibleCategories = _categoryService.GetAll();
        IEnumerable<Manifacturer> allPossibleManifacturers = _manifacturerService.GetAll();

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productData.Id);

        return new IndexProductTableRowPartialModel(
            productData,
            htmlElementId,
            tableIndex,
            ProductSelectListItemUtils.GetCategorySelectListItems(productData, allPossibleCategories),
            ProductSelectListItemUtils.GetManifacturerSelectListItems(productData, allPossibleManifacturers),
            ProductSelectListItemUtils.GetStatusSelectListItems(productData),
            ProductSelectListItemUtils.GetCurrencySelectListItems(productData),
            ProductSelectListItemUtils.GetProductNewStatusSelectListItems(productWorkStatuses),
            ProductSelectListItemUtils.GetProductXmlStatusSelectListItems(productWorkStatuses));
    }




























    private Product? GetProductFull(int productId)
    {
        if (productId <= 0) return null;

        Product? product = _productService.GetByIdWithImages((uint)productId);

        if (product is null) return null;

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct((uint)productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct((uint)productId)
                .ToList();
        }

        return product;
    }

    public IStatusCodeActionResult SaveProductFull(Product productToUpdate)
    {
        if (productToUpdate is null) return BadRequest();

        try
        {
            IStatusCodeActionResult result = _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                () => SaveProductInternal(productToUpdate),
                actionResult => actionResult.StatusCode == 200);

            if (result.StatusCode != 200)
            {
                return result;
            }

            return new OkResult();
        }
        catch (TransactionException)
        {
            return StatusCode(500);
        }
    }

    private IStatusCodeActionResult SaveProductInternal(Product productToUpdate)
    {
        if (productToUpdate is null) return BadRequest();

        uint productId = (uint)productToUpdate.Id;

        Product? oldProduct = GetProductFull(productToUpdate.Id);

        if (oldProduct is null) return BadRequest();
        ProductUpdateRequest productUpdateRequest = MapToUpdateRequestWithoutImagesAndProps(productToUpdate);

        List<(ProductImage productImage, int displayOrder)> ordersForImagesInProduct = new();

        productToUpdate.Images = productToUpdate.Images?.OrderBy(
            image =>
            {
                ProductImageFileNameInfo? relatedFileName = productToUpdate.ImageFileNames?.Find(
                    x => x.FileName == $"{image.Id}.{GetImageFileExtensionFromContentType(image.ImageFileExtension ?? "*")}");

                if (relatedFileName is not null) return relatedFileName.DisplayOrder;

                int imageIndex = productToUpdate.Images!.IndexOf(image);

                ordersForImagesInProduct.Add((image, imageIndex + 1));

                return imageIndex;
            })
            .ToList();

        IStatusCodeActionResult propertiesUpdateActionResult = UpdateProperties(
            productToUpdate, oldProduct, productId, productUpdateRequest);

        if (propertiesUpdateActionResult.StatusCode != 200)
        {
            return propertiesUpdateActionResult;
        }

        IStatusCodeActionResult imagesUpdateActionResult = UpdateImages(
            productToUpdate, oldProduct, productId, productUpdateRequest, ordersForImagesInProduct);

        if (imagesUpdateActionResult.StatusCode != 200)
        {
            return imagesUpdateActionResult;
        }

        IStatusCodeActionResult imageFileNamesUpdateActionResult = UpdateImageFileNames(
            productToUpdate, oldProduct, productId, ordersForImagesInProduct);

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

    private static ProductUpdateRequest MapToUpdateRequestWithoutImagesAndProps(Product productToUpdate)
    {
        return new()
        {
            Id = productToUpdate.Id,
            Name = productToUpdate.Name,
            AdditionalWarrantyPrice = productToUpdate.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productToUpdate.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productToUpdate.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productToUpdate.StandardWarrantyTermMonths,
            DisplayOrder = productToUpdate.DisplayOrder,
            Status = productToUpdate.Status,
            PlShow = productToUpdate.PlShow,
            DisplayPrice = productToUpdate.Price,
            Currency = productToUpdate.Currency,
            RowGuid = productToUpdate.RowGuid,
            Promotionid = productToUpdate.Promotionid,
            PromRid = productToUpdate.PromRid,
            PromotionPictureId = productToUpdate.PromotionPictureId,
            PromotionExpireDate = productToUpdate.PromotionExpireDate,
            AlertPictureId = productToUpdate.AlertPictureId,
            AlertExpireDate = productToUpdate.AlertExpireDate,
            PriceListDescription = productToUpdate.PriceListDescription,
            PartNumber1 = productToUpdate.PartNumber1,
            PartNumber2 = productToUpdate.PartNumber2,
            SearchString = productToUpdate.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryID = productToUpdate.CategoryID,
            ManifacturerId = productToUpdate.ManifacturerId,
            SubCategoryId = productToUpdate.SubCategoryId,
        };
    }

    private IStatusCodeActionResult UpdateProperties(
        Product productToUpdate,
        Product oldFirstProduct,
        uint productId,
        ProductUpdateRequest productUpdateRequestToAddPropsTo)
    {
        if (productToUpdate is null) return BadRequest();

        foreach (ProductProperty newProductProp in productToUpdate.Properties)
        {
            ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
                .Find(prop => prop.ProductCharacteristicId == newProductProp.ProductCharacteristicId);

            if (oldProductPropForSameCharacteristic is null)
            {
                ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
                {
                    ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                    ProductId = (int)productId,
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
                productId, (uint)oldPropertyToDelete.ProductCharacteristicId.Value);

            if (!imageFileNameDeleteSuccess) return StatusCode(500);
        }

        return new OkResult();
    }

    private IStatusCodeActionResult UpdateImages(
        Product productToUpdate,
        Product oldProduct,
        uint productId,
        ProductUpdateRequest productUpdateRequestToAddImagesTo,
        List<(ProductImage productImage, int displayOrder)> ordersForImagesInProduct)
    {
        if (productToUpdate is null) return BadRequest();

        if (productToUpdate.Images is not null)
        {
            ProductImage? productFirstImage = null;

            foreach (ProductImage image in productToUpdate.Images)
            {
                ProductImage? imageInOldProduct = oldProduct.Images?.Find(
                    img => img.Id == image.Id);

                if (imageInOldProduct is null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = (int)productId,
                        ImageData = image.ImageData,
                        ImageFileExtension = image.ImageFileExtension,
                        HtmlData = image.HtmlData,
                    };

                    OneOf<uint, ValidationResult, UnexpectedFailureResult> imageInsertResult
                        = _productImageService.InsertInAllImages(productImageCreateRequest);

                    int imageId = -1;

                    IStatusCodeActionResult actionResultFromImageInsert = imageInsertResult.Match<IStatusCodeActionResult>(
                        id =>
                        {
                            imageId = (int)id;

                            return new OkResult();
                        },
                        validationResult => BadRequest(validationResult),
                        unexpectedFailureResult => StatusCode(500));

                    if (actionResultFromImageInsert.StatusCode != 200)
                    {
                        return actionResultFromImageInsert;
                    }

                    int imageInOrderedListIndex = ordersForImagesInProduct.FindIndex(
                        x => x.productImage == image);

                    if (imageInOrderedListIndex < 0) return StatusCode(500);

                    if (ordersForImagesInProduct[imageInOrderedListIndex].displayOrder == 1)
                    {
                        productFirstImage = image;
                    }

                    (ProductImage productImage, int displayOrder) = ordersForImagesInProduct[imageInOrderedListIndex];

                    productImage.Id = imageId;

                    ordersForImagesInProduct[imageInOrderedListIndex] = new(productImage, displayOrder);

                    continue;
                }

                int imageIndexInOrderedList = ordersForImagesInProduct.FindIndex(x => x.productImage == image);

                if (imageIndexInOrderedList >= 0)
                {
                    int imageDisplayOrderInOrderedList = ordersForImagesInProduct[imageIndexInOrderedList].displayOrder;

                    if (imageDisplayOrderInOrderedList == 1)
                    {
                        productFirstImage = image;
                    }
                }

                if (CompareByteArrays(image.ImageData, imageInOldProduct.ImageData)
                    && image.ImageFileExtension == imageInOldProduct.ImageFileExtension
                    && image.HtmlData == imageInOldProduct.HtmlData)
                {
                    oldProduct.Images?.Remove(imageInOldProduct);

                    continue;
                }

                CurrentProductImageUpdateRequest productImageUpdateRequest = new()
                {
                    ImageData = image.ImageData,
                    ImageFileExtension = image.ImageFileExtension,
                    XML = image.HtmlData,
                };

                productUpdateRequestToAddImagesTo.Images ??= new();

                productUpdateRequestToAddImagesTo.Images.Add(productImageUpdateRequest);

                oldProduct.Images?.Remove(imageInOldProduct);
            }

            if (productFirstImage is not null)
            {
                ProductImage? oldProductFirstImage = _productImageService.GetFirstImageForProduct(productId);

                if (oldProductFirstImage is null)
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult = _productImageService.InsertInFirstImages(new()
                    {
                        ProductId = (int)productId,
                        ImageData = productFirstImage.ImageData,
                        ImageFileExtension = productFirstImage.ImageFileExtension,
                        HtmlData = productFirstImage.HtmlData,
                    });

                    IStatusCodeActionResult actionResultFromFirstImageInsert = insertFirstImageResult.Match<IStatusCodeActionResult>(
                        success => new OkResult(),
                        validationResult => BadRequest(validationResult),
                        unexpectedFailureResult => StatusCode(500));

                    if (actionResultFromFirstImageInsert.StatusCode != 200) return actionResultFromFirstImageInsert;
                }
                else
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFirstImageResult = _productImageService.UpdateInFirstImages(new()
                    {
                        ProductId = (int)productId,
                        ImageData = productFirstImage.ImageData,
                        ImageFileExtension = productFirstImage.ImageFileExtension,
                        HtmlData = productFirstImage.HtmlData,
                    });

                    IStatusCodeActionResult actionResultFromFirstImageInsert = updateFirstImageResult.Match<IStatusCodeActionResult>(
                       success => new OkResult(),
                       validationResult => BadRequest(validationResult),
                       unexpectedFailureResult => StatusCode(500));

                    if (actionResultFromFirstImageInsert.StatusCode != 200) return actionResultFromFirstImageInsert;
                }
            }
        }

        if (oldProduct.Images is not null
            && oldProduct.Images.Count > 0)
        {
            foreach (ProductImage oldImageToBeRemoved in oldProduct.Images)
            {
                bool imageDeleteResult = _productImageService.DeleteInAllImagesById((uint)oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return StatusCode(500);
            }
        }

        return new OkResult();
    }

    private IStatusCodeActionResult UpdateImageFileNames(
        Product productToUpdate,
        Product oldProduct,
        uint productId,
        List<(ProductImage productImage, int displayOrder)> ordersForImagesInProduct)
    {
        if (productToUpdate is null) return BadRequest();

        List<ServiceProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests = new();

        if (oldProduct.ImageFileNames is not null
            && oldProduct.ImageFileNames.Count > 0)
        {
            if (productToUpdate.ImageFileNames is null
                || productToUpdate.ImageFileNames.Count <= 0)
            {
                foreach (ProductImageFileNameInfo oldProductImageFileName in oldProduct.ImageFileNames)
                {
                    if (oldProductImageFileName.DisplayOrder is null) return StatusCode(500);

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return StatusCode(500);

                    for (int k = 0; k < oldProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldProduct.ImageFileNames.Count; i++)
                {
                    ProductImageFileNameInfo oldProductImageFileName = oldProduct.ImageFileNames[i];

                    if (oldProductImageFileName.DisplayOrder is null) return StatusCode(500);

                    ProductImageFileNameInfo? oldImageFileNameInCurrentProduct = productToUpdate.ImageFileNames.Find(
                        imgFileNameInfo => imgFileNameInfo.FileName == oldProductImageFileName.FileName);

                    if (oldImageFileNameInCurrentProduct is not null) continue;

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return StatusCode(500);

                    oldProduct.ImageFileNames.RemoveAt(i);

                    i--;

                    for (int k = 0; k < oldProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
        }

        if (productToUpdate.ImageFileNames is not null)
        {
            int newImageFileNamesCount = 0;

            foreach (ProductImageFileNameInfo imageFileNameInfo in productToUpdate.ImageFileNames)
            {
                ProductImageFileNameInfo? imageFileNameOfOldProduct = oldProduct.ImageFileNames?.Find(
                    imgFileNameInfo => imgFileNameInfo.FileName == imageFileNameInfo.FileName);

                if (imageFileNameOfOldProduct is null)
                {
                    ProductImage? imageRelatedToThatFileNameInfoLocal = ordersForImagesInProduct.Find(
                        x => imageFileNameInfo.DisplayOrder == x.displayOrder)
                        .productImage;

                    if (imageRelatedToThatFileNameInfoLocal.Id <= 0
                        || string.IsNullOrWhiteSpace(imageRelatedToThatFileNameInfoLocal.ImageFileExtension)) return StatusCode(500);

                    if (imageRelatedToThatFileNameInfoLocal is null) return StatusCode(500);

                    ServiceProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = new()
                    {
                        ProductId = productToUpdate.Id,
                        DisplayOrder = imageFileNameInfo.DisplayOrder,
                        Active = imageFileNameInfo.Active,
                        FileName = $"{imageRelatedToThatFileNameInfoLocal.Id}.{GetImageFileExtensionFromContentType(imageRelatedToThatFileNameInfoLocal.ImageFileExtension ?? "*")}",
                    };

                    imageFileNameInfoCreateRequests.Add(imageFileNameCreateRequest);

                    newImageFileNamesCount++;

                    continue;
                }

                if (imageFileNameInfo.FileName is null
                    || imageFileNameInfo.DisplayOrder is null) continue;

                ProductImage? imageRelatedToThatFileNameInfo = ordersForImagesInProduct.Find(
                    x => imageFileNameInfo.DisplayOrder == x.displayOrder)
                    .productImage;

                if (imageRelatedToThatFileNameInfo.Id <= 0
                    || string.IsNullOrWhiteSpace(imageRelatedToThatFileNameInfo.ImageFileExtension)) return StatusCode(500);

                string? temporaryId = GetTemporaryIdFromFileNameInfoAndContentType(imageFileNameInfo, imageRelatedToThatFileNameInfo.ImageFileExtension);

                if (temporaryId is not null)
                {
                    string? fullFileNameFromImageData = GetImageFileNameFromImageData(
                        imageRelatedToThatFileNameInfo.Id, imageRelatedToThatFileNameInfo.ImageFileExtension);

                    if (fullFileNameFromImageData is null) return StatusCode(500);

                    imageFileNameInfo.FileName = fullFileNameFromImageData;
                }

                int displayOrder = imageFileNameInfo.DisplayOrder.Value - newImageFileNamesCount;

                bool isFileNameInfoSameAsOldOne = imageFileNameInfo.FileName == imageFileNameOfOldProduct.FileName
                    && displayOrder == imageFileNameOfOldProduct.DisplayOrder
                    && imageFileNameInfo.Active == imageFileNameOfOldProduct.Active;

                if (isFileNameInfoSameAsOldOne) continue;

                ServiceProductImageFileNameInfoByFileNameUpdateRequest imageFileNameUpdateRequest = new()
                {
                    ProductId = (int)productId,
                    NewDisplayOrder = displayOrder,
                    Active = imageFileNameInfo.Active,
                    FileName = imageFileNameOfOldProduct.FileName!,
                    NewFileName = imageFileNameInfo.FileName,
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

    private static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.SequenceEqual(b);
    }
}