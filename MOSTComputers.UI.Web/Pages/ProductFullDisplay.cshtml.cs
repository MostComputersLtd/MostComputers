using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;

namespace MOSTComputers.UI.Web.Pages;

public class ProductFullDisplayModel : PageModel
{
    public ProductFullDisplayModel(
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductImageService productImageService,
        IProductCharacteristicService productCharacteristicService,
        ISearchStringOriginService searchStringOriginService)
    {
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _productCharacteristicService = productCharacteristicService;
        _searchStringOriginService = searchStringOriginService;
    }

    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageService _productImageService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly ISearchStringOriginService _searchStringOriginService;

    public Product? Product { get; private set; }
    public List<ProductCharacteristic>? ProductCharacteristics { get; private set; }
    public List<ProductProperty>? ProductProperties { get; private set; }
    public List<ProductImageData>? ProductImages { get; private set; }
    public List<SearchStringPartOriginData>? SearchStringParts { get; private set; }

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return Page();
        }

        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        IEnumerable<ProductCharacteristicType> productCharacteristicTypes = [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link];

        List<ProductCharacteristic> productCharacteristics = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(productId);
        List<ProductImageData> productImages = await _productImageService.GetAllInProductWithoutFileDataAsync(productId);

        List<SearchStringPartOriginData>? searchStringParts
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(product.SearchString, product.CategoryId);

        Product = product;
        ProductCharacteristics = productCharacteristics;
        ProductProperties = productProperties;
        ProductImages = productImages;
        SearchStringParts = searchStringParts;

        return Page();
    }
}
