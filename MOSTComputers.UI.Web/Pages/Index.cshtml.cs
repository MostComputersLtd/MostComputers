using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.ProductSearch;
using MOSTComputers.UI.Web.Pages.Shared.Index;
using MOSTComputers.UI.Web.Services.Data.Search.Contracts;
using OneOf;

using static MOSTComputers.UI.Web.Utils.SelectListItemUtils;
using static MOSTComputers.Utils.OneOf.AsyncMatchingExtensions;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public IndexModel(ICategoryService categoryService,
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        ISearchStringOriginService searchStringOriginService,
        IProductHtmlService productHtmlService,
        IProductSearchService productSearchService)
    {
        _categoryService = categoryService;
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productCharacteristicService = productCharacteristicService;
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
        _searchStringOriginService = searchStringOriginService;
        _productHtmlService = productHtmlService;
        _productSearchService = productSearchService;
    }

    public readonly string ProductTablePartialContainerElementId = "productTablePartialContainer";

    private const string _productTablePartialPath = "Index/_ProductsTablePartial";
    private const string _productDataPopupPartialPath = "Index/_ProductFullDisplayPartial";
    private const string _productHtmlAndImagesPartialPath = "Index/_ProductHtmlAndImagesPartial";

    public readonly ModalData ProductFullDisplayPartialModalData = new()
    {
        ModalId = "productsModal",
        ModalDialogId = "productsModalDialog",
        ModalContentId = "productsModalContent",
    };

	public readonly ModalData ProductHtmlAndImagesPartialModalData = new()
	{
		ModalId = "productHtmlAndImagesModal",
		ModalDialogId = "productHtmlAndImagesModalDialog",
		ModalContentId = "productHtmlAndImagesModalContent",
	};

	private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly IProductSearchService _productSearchService;

    public void OnGet()
    {
	}

    public async Task<IActionResult> OnGetGetProductDataPopupAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return NotFound();
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

        ProductFullDisplayPartialModel model = new()
        {
            ModalData = ProductFullDisplayPartialModalData,
            Product = product,
            ProductCharacteristics = productCharacteristics,
            ProductProperties = productProperties,
            ProductImages = productImages,
            SearchStringParts = searchStringParts,
        };

        return Partial(_productDataPopupPartialPath, model);
    }

    public async Task<IActionResult> OnGetGetProductHtmlAndImagesPopupAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        List<HtmlProductProperty> htmlProductProperties = new();

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(productId);

        foreach (ProductProperty productProperty in productProperties)
        {
            HtmlProductProperty htmlProductProperty = new()
            {
                Name = productProperty.Characteristic,
                Value = productProperty.Value,
                Order = productProperty.DisplayOrder?.ToString(),
            };

            htmlProductProperties.Add(htmlProductProperty);
        }

        HtmlProduct htmlProduct = new()
        {
            Id = product.Id,
            Name = product.Name,
            Properties = htmlProductProperties,
        };

        HtmlProductsData htmlProductsData = new()
        {
            Products = [htmlProduct]
        };
        
        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(htmlProductsData);

        return await getProductHtmlResult.MatchAsync<string, InvalidXmlResult, IStatusCodeActionResult>(
            async productHtml =>
            {
                List<ProductImageData> productImages = await _productImageService.GetAllInProductWithoutFileDataAsync(productId);
                List<ProductImageFileData> productImageFiles = await _productImageFileService.GetAllInProductAsync(productId);

                ProductHtmlAndImagesPartialModel model = new()
                {
                    ModalData = ProductHtmlAndImagesPartialModalData,
                    Product = product,
                    ProductImages = productImages,
                    ProductImageFileNames = productImageFiles,
                    ProductHtml = productHtml,
                };

                return Partial(_productHtmlAndImagesPartialPath, model);
            },
            invalidXmlResult => BadRequest());
    }

    public async Task<IActionResult> OnPostGetSearchedProductsAsync([FromBody] ProductSearchRequest? productSearchRequest = null)
    {
        if (productSearchRequest is null)
        {
            ProductsTablePartialModel modelInner = new()
            {
                ProductFullDisplayPartialModalData = ProductFullDisplayPartialModalData,
                ProductHtmlAndImagesPartialModelData = ProductHtmlAndImagesPartialModalData,
            };

            return Partial(_productTablePartialPath, modelInner);
        }

        List<Product> searchedProducts = await _productSearchService.SearchProductsAsync(productSearchRequest);

        ProductsTablePartialModel model = new()
        {
            ProductFullDisplayPartialModalData = ProductFullDisplayPartialModalData,
            ProductHtmlAndImagesPartialModelData = ProductHtmlAndImagesPartialModalData,
            Products = searchedProducts,
        };

        return Partial(_productTablePartialPath, model);
    }

    public async Task<List<SelectListItem>> GetAllCategorySelectListItemsAsync(int? selectedCategoryId = null)
    {
        List<Category> categories = await _categoryService.GetAllAsync();

        bool isAnySelected = selectedCategoryId is not null;

        List<SelectListItem> selectListItems = GetCategorySelectListItems(categories, selectedCategoryId, new("Всички", null, isAnySelected, true));

        return selectListItems;
    }
}