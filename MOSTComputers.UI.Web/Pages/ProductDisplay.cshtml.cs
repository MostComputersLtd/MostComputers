using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.UI.Web.Pages.Shared;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.StaticUtilities;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Pages;

public sealed class ProductDisplayModel : PageModel
{
    public ProductDisplayModel(
        ICategoryService categoryService,
        IProductXmlToCreateRequestMappingService mapperService,
        IProductDeserializeService productDeserializeService,
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductStatusesService productStatusesService,
        IProductImageService productImageService,
        ISearchStringOriginService searchStringOriginService)
    {
        _categoryService = categoryService;
        _mapperService = mapperService;
        _productDeserializeService = productDeserializeService;
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productStatusesService = productStatusesService;
        _productImageService = productImageService;
        _searchStringOriginService = searchStringOriginService;

        AllCategoriesSelectItems = InitializeCategorySelectItems();

        if (ProductsAndStatuses is null
            || ProductsAndStatuses.Count <= 0)
        {
            PopulateProductsFromStartToEnd(0, 12);
        }
    }

    private IEnumerable<SelectListItem> InitializeCategorySelectItems()
    {
        IEnumerable<SelectListItem> output = _categoryService.GetAll()
            .Select(category =>
            new SelectListItem()
            {
                Text = category.Description,
                Value = category.Id.ToString(),
            });

        return output.Prepend(new() { Text = "All", Value = "null", Selected = true });
    }

    private void PopulateProductsFromStartToEnd(uint start, uint length)
    {
        List<Product> products = _productService.GetFirstItemsBetweenStartAndEnd(new() { Start = start, Length = length })
            .ToList();

        IEnumerable<ProductStatuses> productStatuses = _productStatusesService.GetSelectionByProductIds(
            products.Select(product => (uint)product.Id));

        ProductsAndStatuses = new();

        foreach (Product product in products)
        {
            ProductAndStatuses item = new()
            {
                Product = product,
                ProductStatuses = productStatuses.FirstOrDefault(x => x.ProductId == product.Id)
                    ?? new() { ProductId = product.Id, IsProcessed = false, NeedsToBeUpdated = false },
                SearchStringPartOriginDataList = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(product)!
            };

            ProductsAndStatuses.Add(item);
        }
    }

    private readonly ICategoryService _categoryService;
    private readonly IProductXmlToCreateRequestMappingService _mapperService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductImageService _productImageService;
    private readonly ISearchStringOriginService _searchStringOriginService;

    // So i dont have to new up lists for range searches
    private readonly List<Product> _singleProductList = new();

    private string? _currentSubStringForSearchStringSearches { get; set; } = null;

    public IEnumerable<SelectListItem> AllCategoriesSelectItems { get; set; }

    public static List<ProductAndStatuses> ProductsAndStatuses { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnGetPartialViewXmlForProduct(uint productId)
    {
        Product? product = GetProductByIdFromCollection(productId, addProps: true, addImageFilePaths: true);

        if (product is null) return BadRequest();

        string? productXml = GetProductXML(product);

        if (productXml is null) return BadRequest();

        return Partial("_ProductGeneratedXmlPopupPartial", new ProductGeneratedXmlPopupPartialModel() { XmlData = productXml, Product = product });
    }

    public IActionResult OnGetPartialViewImagesForProduct(uint productId)
    {
        Product? product = GetProductByIdFromCollection(productId, addImages: true, addImageFilePaths: true);

        if (product is null) return BadRequest();

        return Partial("_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(product));
    }

    public IActionResult OnGetCurrentImageFileResultSingle(uint productId, uint imageIndex)
    {
        Product? product = GetProductByIdFromCollection(productId);

        if (product is null
            || product.Images is null
            || product.Images.Count <= imageIndex) return BadRequest();

        ProductImage image = product.Images[(int)imageIndex];

        if (image.ImageData is null
            || image.ImageFileExtension is null) return StatusCode(500);

        int slashIndex = image.ImageFileExtension.IndexOf('/');

        string fileExtension = image.ImageFileExtension[(slashIndex + 1)..];

        return File(image.ImageData, image.ImageFileExtension, $"{productId}_{imageIndex}.{fileExtension}");
    }

    public IActionResult OnGetSearchProductById(uint? productId)
    {
        if (productId is null
            || productId == 0)
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new OkResult();
        }
        ProductsAndStatuses = new();

        Product? product = _productService.GetByIdWithFirstImage(productId.Value);

        if (product is null) return BadRequest();

        ProductStatuses? productStatuses = _productStatusesService.GetByProductId(productId.Value);

        ProductAndStatuses item = new()
        {
            Product = product,
            ProductStatuses = productStatuses ?? new() { ProductId = (int)productId, IsProcessed = false, NeedsToBeUpdated = false }
        };

        ProductsAndStatuses.Add(item);

        return new OkResult();
    }

    public IActionResult OnGetSearchProductWhereSearchStringMatches(string? subString)
    {
        _currentSubStringForSearchStringSearches = subString;

        if (string.IsNullOrWhiteSpace(subString)
            || subString.Length == 0)
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        ProductsAndStatuses = new();

        IEnumerable<Product> products = _productService.GetFirstInRangeWhereSearchStringMatches(new() { Start = 0, Length = 12 }, subString);

        if (!products.Any())
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        IEnumerable<uint> productIds = products.Select(product => (uint)product.Id);

        IEnumerable<ProductStatuses> productStatuses = _productStatusesService.GetSelectionByProductIds(productIds);

        foreach (Product product in products)
        {
            ProductStatuses? productStatusesForProduct = productStatuses.FirstOrDefault(x => x.ProductId == product.Id);

            ProductAndStatuses item = new()
            {
                Product = product,
                ProductStatuses = productStatusesForProduct ?? new() { ProductId = product.Id, IsProcessed = false, NeedsToBeUpdated = false }
            };

            ProductsAndStatuses.Add(item);
        }

        return new JsonResult(new { length = products.Count() });
    }

    public IActionResult OnGetSearchProductWhereNameMatches(string? subString)
    {
        _currentSubStringForSearchStringSearches = subString;

        if (string.IsNullOrWhiteSpace(subString)
            || subString.Length == 0)
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        ProductsAndStatuses = new();

        IEnumerable<Product> products = _productService.GetFirstInRangeWhereNameMatches(new() { Start = 0, Length = 12 }, subString);

        if (!products.Any())
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        IEnumerable<uint> productIds = products.Select(product => (uint)product.Id);

        IEnumerable<ProductStatuses> productStatuses = _productStatusesService.GetSelectionByProductIds(productIds);

        foreach (Product product in products)
        {
            ProductStatuses? productStatusesForProduct = productStatuses.FirstOrDefault(x => x.ProductId == product.Id);

            ProductAndStatuses item = new()
            {
                Product = product,
                ProductStatuses = productStatusesForProduct ?? new() { ProductId = product.Id, IsProcessed = false, NeedsToBeUpdated = false }
            };

            ProductsAndStatuses.Add(item);
        }

        return new JsonResult(new { length = products.Count() });
    }

    public IActionResult OnPostSearchProductWhereAllConditionsMatch([FromBody] ProductConditionalSearchRequestDTO data)
    {
        if (data.StatusInt is null
            && data.NameSubstring is null
            && data.SearchStringSubstring is null
            && data.CategoryId is null
            && data.IsProcessed is null
            && data.NeedsToBeUpdated is null)
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        bool successForIsProcessed = TryParseNullableBool(data.IsProcessed, out bool? isProcessed);

        if (!successForIsProcessed)
        {
            return StatusCode(500);
        }

        bool successForNeedsToBeUpdated = TryParseNullableBool(data.IsProcessed, out bool? needsToBeUpdated);

        if (!successForNeedsToBeUpdated)
        {
            return StatusCode(500);
        }

        ProductStatusEnum? productStatusEnum = (ProductStatusEnum?)data.StatusInt;


        ProductsAndStatuses = new();

        ProductRangeSearchRequest productRangeSearchRequest = new()
        {
            Start = 0,
            Length = 12,
        };

        ProductConditionalSearchRequest productConditionalSearchRequest = new()
        {
            Status = productStatusEnum,
            NameSubstring = data.NameSubstring,
            SearchStringSubstring = data.SearchStringSubstring,
            CategoryId = data.CategoryId,
            IsProcessed = isProcessed,
            NeedsToBeUpdated = needsToBeUpdated,
        };

        IEnumerable<Product>? products = GetProductsForSearchStringData(data, productRangeSearchRequest, productConditionalSearchRequest);

        if (!products!.Any())
        {
            PopulateProductsFromStartToEnd(0, 12);

            return new JsonResult(new { length = 12 });
        }

        IEnumerable<uint> productIds = products!.Select(product => (uint)product.Id);

        IEnumerable<ProductStatuses> productStatuses = _productStatusesService.GetSelectionByProductIds(productIds);

        foreach (Product product in products!)
        {
            ProductStatuses? productStatusesForProduct = productStatuses.FirstOrDefault(x => x.ProductId == product.Id);

            ProductAndStatuses item = new()
            {
                Product = product,
                ProductStatuses = productStatusesForProduct ?? new() { ProductId = product.Id, IsProcessed = false, NeedsToBeUpdated = false },
                SearchStringPartOriginDataList = _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(product)
            };

            ProductsAndStatuses.Add(item);
        }

        return new JsonResult(new { length = products.Count() });
    }

    private IEnumerable<Product> GetProductsForSearchStringData(ProductConditionalSearchRequestDTO data, ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        IEnumerable<Product>? products = null;

        if (data.SearchStringSubstring is null)
        {
            products = _productService.GetFirstInRangeWhereAllConditionsAreMet(productRangeSearchRequest, productConditionalSearchRequest);

            return products;
        }

        (string firstSearchString, string? secondSearchString) = GetSearchStrings(data.SearchStringSubstring);

        productConditionalSearchRequest.SearchStringSubstring = firstSearchString;

        IEnumerable<Product> products1 = _productService.GetFirstInRangeWhereAllConditionsAreMet(productRangeSearchRequest, productConditionalSearchRequest);

        if (!string.IsNullOrWhiteSpace(secondSearchString))
        {
            productConditionalSearchRequest.SearchStringSubstring = secondSearchString;

            IEnumerable<Product> products2 = _productService.GetFirstInRangeWhereAllConditionsAreMet(productRangeSearchRequest, productConditionalSearchRequest);

            products = products1.Union(products2)
                .OrderBy(x => x.DisplayOrder)
                .Take(12);

            return products;
        }

        return products1;
    }

    private static (string firstSearchString, string? secondSearchString) GetSearchStrings(string searchStringData)
    {
        int indexOfParenthesesInSearchStringSubstr = searchStringData.IndexOf('[');

        if (indexOfParenthesesInSearchStringSubstr == -1) return (searchStringData, null);

        int endIndexOfParenthesesInSearchStringSubstr = searchStringData.IndexOf(']');

        if (endIndexOfParenthesesInSearchStringSubstr == -1) return (searchStringData, null);

        string firstSearchData = searchStringData[..(indexOfParenthesesInSearchStringSubstr - 1)]
            .Trim();

        int secondSearchStringDataLength = endIndexOfParenthesesInSearchStringSubstr - indexOfParenthesesInSearchStringSubstr - 1;

        string secondSearchData = searchStringData.Substring(indexOfParenthesesInSearchStringSubstr + 1, secondSearchStringDataLength)
            .Trim();

        string endString = searchStringData[(endIndexOfParenthesesInSearchStringSubstr + 1)..]
            .Trim();

        string? firstSearchString = $"{firstSearchData} {endString}";
        string? secondSearchString = $"{secondSearchData} {endString}";

        return (firstSearchString, secondSearchString);
    }

    private static bool TryParseNullableBool(string? value, out bool? result)
    {
        if (value is null)
        {
            result = null;

            return true;
        }

        bool success = bool.TryParse(value, out bool resultLocal);

        if (success)
        {
            result = resultLocal;

            return true;
        }

        result = null;

        return false;
    }

    public IActionResult OnGetGetTablePartialView()
    {
        return Partial("_ProductsAndStatusesDisplayPartialTable",
            new ProductsAndStatusesDisplayPartialTableModel()
            {
                ProductsAndStatuses = ProductsAndStatuses,
                AllCategoriesSelectItems = AllCategoriesSelectItems,
                CurrentSubStringForSearchStringSearches = _currentSubStringForSearchStringSearches,
            });
    }

    public IActionResult OnPostAddNewImageToProduct(uint productId, IFormFile fileInfo)
    {
        using MemoryStream stream = new();

        fileInfo.CopyTo(stream);

        byte[] imageBytes = stream.ToArray();

        string contentType = fileInfo.ContentType;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertImageResult = InsertImageWithData(productId, imageBytes, contentType);

        OneOf<int, IActionResult> getIdOfInsertedImageResult = insertImageResult.Match<OneOf<int, IActionResult>>(
            id => (int)id,
            _ => StatusCode(500),
            _ => BadRequest());

        if (getIdOfInsertedImageResult.IsT1) return getIdOfInsertedImageResult.AsT1;

        //int idOfInsertedImage = getIdOfInsertedImageResult.AsT0;

        Product? product = GetProductByIdFromCollection(productId, addImages: true, addImageFilePaths: true);

        if (product is null) return BadRequest();

        return Partial("_ProductImagesDisplayPopupPartial", new ProductImagesDisplayPopupPartialModel(product));
    }

    private OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertImageWithData(uint productId, byte[] imageBytes, string contentType)
    {
        ServiceProductImageCreateRequest imageCreateRequest = new()
        {
            ImageData = imageBytes,
            ImageFileExtension = contentType,
            ProductId = (int)productId,
            XML = GetProductXML(productId)
        };

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertImageResult = _productImageService.InsertInAllImagesAndImageFileNameInfos(imageCreateRequest, null);

        return insertImageResult;
    }

    public IActionResult OnPutUpdateImageOrder(uint productId, uint oldDisplayOrder, uint newDisplayOrder)
    {
        if (oldDisplayOrder == newDisplayOrder) return new OkResult();

        IEnumerable<ProductImageFileNameInfo> imageFileNameInfosForProduct
            = _productImageFileNameInfoService.GetAllInProduct(productId);

        ProductImageFileNameInfo? imageFileNameInfoThatWasUpdated = imageFileNameInfosForProduct.FirstOrDefault(x => x.DisplayOrder == oldDisplayOrder);

        if (imageFileNameInfoThatWasUpdated is null) return BadRequest();

        ServiceProductImageFileNameInfoUpdateRequest fileNameInfoUpdateRequest = new()
        {
            DisplayOrder = (int)oldDisplayOrder,
            NewDisplayOrder = (int)newDisplayOrder,
            FileName = imageFileNameInfoThatWasUpdated.FileName,
            ProductId = imageFileNameInfoThatWasUpdated.ProductId,
            Active = imageFileNameInfoThatWasUpdated.Active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoUpdateResult
            = _productImageFileNameInfoService.Update(fileNameInfoUpdateRequest);

        IStatusCodeActionResult statusCodeActionResult = imageFileNameInfoUpdateResult.Match<IStatusCodeActionResult>(
            _ => new OkResult(),
            validationResult => BadRequest(validationResult),
            _ => StatusCode(500));

        if (statusCodeActionResult.StatusCode != 200) return statusCodeActionResult;

        if (oldDisplayOrder == 1)
        {
            ProductImageFileNameInfo newFirstImageFileInfo = imageFileNameInfosForProduct.ElementAt(1);

            if (newFirstImageFileInfo is null) return StatusCode(500);

            return ReplaceFirstImageWithNewOneFromImageFileName(productId, newFirstImageFileInfo.FileName);
        }
        else if (newDisplayOrder == 1)
        {
            return ReplaceFirstImageWithNewOneFromImageFileName(productId, imageFileNameInfoThatWasUpdated.FileName);
        }

        return statusCodeActionResult;

        IActionResult ReplaceFirstImageWithNewOneFromImageFileName(uint productId, string? fileName)
        {
            ProductImage? newFirstImage = GetImageFromFileNameInfoName(fileName);

            if (newFirstImage is null) return StatusCode(500);

            ServiceProductFirstImageCreateRequest imageCreateRequest = GetFirstImageCreateRequestFromImage(newFirstImage);

            bool removeFirstImageResult = _productImageService.DeleteInFirstImagesByProductId(productId);

            if (!removeFirstImageResult) return StatusCode(500);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> addNewFirstImageResult
                = _productImageService.InsertInFirstImages(imageCreateRequest);

            return addNewFirstImageResult.Match<IActionResult>(
                _ => new OkResult(),
                validationResult => BadRequest(validationResult),
                _ => StatusCode(500));
        }
    }

    private static ServiceProductFirstImageCreateRequest GetFirstImageCreateRequestFromImage(ProductImage newFirstImage)
    {
        return new()
        {
            ProductId = (int)newFirstImage.ProductId!,
            ImageData = newFirstImage.ImageData,
            ImageFileExtension = newFirstImage.ImageFileExtension,
            XML = newFirstImage.XML,
        };
    }

    private ProductImage? GetImageFromFileNameInfoName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;

        int? idOfImage = ImageFileNameInfoToImageDataUtils.GetImageIdFromImageFileNameInfoName(fileName);

        if (idOfImage is null) return null;

        ProductImage? newFirstImage = _productImageService.GetByIdInAllImages((uint)idOfImage);

        return newFirstImage;
    }

    public IActionResult OnPutUpdateImageActiveStatus(uint productId, uint displayOrder, string fileName, bool active)
    {
        ServiceProductImageFileNameInfoUpdateRequest updateRequest = new()
        {
            ProductId = (int)productId,
            DisplayOrder = (int)displayOrder,
            FileName = fileName,
            Active = active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImageFileNameInfoResult = _productImageFileNameInfoService.Update(updateRequest);

        return updateImageFileNameInfoResult.Match<IActionResult>(
            _ => new OkResult(),
            validationResult => BadRequest(validationResult),
            _ => StatusCode(500));
    }

    public IActionResult OnDeleteDeleteImageFromProduct(uint imageId)
    {
        bool deleteImageResult = _productImageService.DeleteInAllImagesAndImageFilePathInfosById(imageId);

        return deleteImageResult ? new OkResult() : BadRequest();
    }

    public string? GetProductXML(uint productId)
    {
        Product? product = GetProductByIdFromCollection(productId);

        if (product is null) return null;

        AddToSingleCollectionList(product);

        XmlObjectData xmlDataFromProduct = _mapperService.GetXmlDataFromProducts(_singleProductList);

        OneOf<string?, InvalidXmlResult> serializationResult = _productDeserializeService.TrySerializeProductsXml(xmlDataFromProduct, true);

        return serializationResult.Match(
            data => data,
            invalidXmlResult => null);
    }

    private string? GetProductXML(Product product)
    {
        AddToSingleCollectionList(product);

        XmlObjectData xmlDataFromProduct = _mapperService.GetXmlDataFromProducts(_singleProductList);

        OneOf<string?, InvalidXmlResult> serializationResult = _productDeserializeService.TrySerializeProductsXml(xmlDataFromProduct, true);

        return serializationResult.Match(
            data => data,
            invalidXmlResult => null);
    }

    private Product? GetProductByIdFromCollection(uint productId, bool addProps = false, bool addImages = false, bool addImageFilePaths = false)
    {
        int productIndex = ProductsAndStatuses.FindIndex(x => x.Product.Id == productId);

        if (productIndex == -1) return null;

        ProductAndStatuses? productWithStatuses = ProductsAndStatuses[productIndex];

        Product product = productWithStatuses.Product;

        if (addProps)
        {
            List<ProductProperty> propsOfProduct = _productPropertyService.GetAllInProduct(productId)
                .ToList();

            if (propsOfProduct.Count > 0)
            {
                product.Properties = propsOfProduct;
            }
        }

        if (addImages)
        {
            List<ProductImage> imagesOfProduct = _productImageService.GetAllInProduct(productId)
            .ToList();

            if (imagesOfProduct.Count > 0)
            {
                product.Images = imagesOfProduct;
            }
        }

        if (addImageFilePaths)
        {
            List<ProductImageFileNameInfo> imageFileNamesOfProduct = _productImageFileNameInfoService.GetAllInProduct(productId)
            .ToList();

            if (imageFileNamesOfProduct.Count > 0)
            {
                product.ImageFileNames = imageFileNamesOfProduct;
            }
        }

        return product;
    }

    private void AddToSingleCollectionList(Product product)
    {
        _singleProductList.Clear();

        _singleProductList.Add(product);
    }
}

public class ProductAndStatuses
{
    public required Product Product { get; set; }
    public required ProductStatuses ProductStatuses { get; set; }
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? SearchStringPartOriginDataList { get; set; }
}

public class ProductConditionalSearchRequestDTO
{
    public int? StatusInt { get; set; }
    public string? NameSubstring { get; set; }
    public string? SearchStringSubstring { get; set; }
    public int? CategoryId { get; set; }
    public string? IsProcessed { get; set; }
    public string? NeedsToBeUpdated { get; set; }
}