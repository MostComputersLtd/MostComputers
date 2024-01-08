using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.UI.Web.Pages.Shared;
using MOSTComputers.UI.Web.Services.Contracts;
using OneOf;

namespace MOSTComputers.UI.Web.Pages;

public sealed class ProductDisplayModel : PageModel
{
    public ProductDisplayModel(
        ICategoryService categoryService,
        IProductXmlToCreateRequestMapperService mapperService,
        IProductDeserializeService productDeserializeService,
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductStatusesService productStatusesService,
        IProductImageService productImageService)
    {
        _categoryService = categoryService;
        _mapperService = mapperService;
        _productDeserializeService = productDeserializeService;
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productStatusesService = productStatusesService;
        _productImageService = productImageService;

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
                    ?? new() { ProductId = product.Id, IsProcessed = false, NeedsToBeUpdated = false }
            };

            ProductsAndStatuses.Add(item);
        }
    }

    private readonly ICategoryService _categoryService;
    private readonly IProductXmlToCreateRequestMapperService _mapperService;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductImageService _productImageService;

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
        Product? product = GetProductByIdFromCollection(productId, addImages: true);

        if (product is null) return BadRequest();

        return Partial("_ProductImagesDisplayPopup", new ProductImagesDisplayPopupModel() { Product = product });
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

        IEnumerable<Product> products = _productService.GetFirstInRangeWhereAllConditionsAreMet(productRangeSearchRequest, productConditionalSearchRequest);

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
            List<ProductImageFileNameInfo> imageFileNamesOfProduct = _productImageFileNameInfoService.GetAllForProduct(productId)
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