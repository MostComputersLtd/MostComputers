using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData;
using MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductListItems;

namespace MOSTComputers.UI.Web.Client.Pages;
public class IndexModel : PageModel
{
    public sealed class ProductSearchOptions
    {
        public int? PageNumber { get; set; }
        public string? SearchData { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public bool? AvailableOnly { get; set; }
        public ProductSearchSortingOptions? SortingMethod { get; set; }
    }

    public enum ProductSearchSortingOptions
    {
        Default = 0,
        PriceDescending = 1,
        PriceAcsending = 2,
        PromotionsFirst = 3,
        Alphabetically = 4,
    }

    private sealed class ManufacturerSearchResponse
    {
        public int? id { get; set; }
        public string? displayName { get; set; }
    }

    public IndexModel(ILogger<IndexModel> logger,
        ICategoryService categoryService,
        IManufacturerService manufacturerService,
        IProductService productService,
        IProductSearchService productSearchService,
        IPromotionService promotionService)
    {
        _logger = logger;
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
        _productService = productService;
        _productSearchService = productSearchService;
        _promotionService = promotionService;
    }

    private const string _dialogId = "productDataDialog";

    internal const string ProductDataComponentPath = "ProductData";
    internal const string ProductTableComponentPath = "ProductTable";

    private const int _searchResultsPageCount = 50;

    private readonly ILogger<IndexModel> _logger;
    private readonly ICategoryService _categoryService;
    private readonly IProductSearchService _productSearchService;
    private readonly IPromotionService _promotionService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IProductService _productService;

    public List<Product> Products { get; set; } = new();

    public List<Category> CategoriesInSelect { get; set; } = new();
    public List<Manufacturer> ManufacturersInSelect { get; set; } = new();

    public List<string> GroupPromotionImageUrls { get; set; } = new();

    public bool IsDefaultManufacturerOptionSelected { get; set; } = true;

    public Currency Currency { get; set; } = Currency.EUR;
    public Currency? SecondaryCurrency { get; set; } = Currency.BGN;

    public async Task OnGetAsync([FromQuery(Name = "page")] int? pageNumber)
    {
        CategoriesInSelect = await _categoryService.GetAllAsync();
        ManufacturersInSelect = await _manufacturerService.GetAllAsync();

        if (pageNumber != null && pageNumber > 0)
        {
            ProductSearchOptions productSearchOptions = new()
            {
                PageNumber = pageNumber,
            };

            Products = await SearchProductsAsync(productSearchOptions);
        }
    }

    public async Task<JsonResult> OnGetGetManufacturersForCategoryAsync(int? categoryId)
    {
        if (categoryId.HasValue)
        {
            ProductSearchRequest productSearchRequest = new()
            {
                OnlyVisibleByEndUsers = true,
                CategoryId = categoryId.Value,
                ProductStatus = ProductStatusSearchOptions.AvailableAndCall,
            };

            List<Product> visibleProductsInCategory = await _productSearchService.SearchProductsAsync(productSearchRequest);

            List<Manufacturer> manufacturersInProducts = new();

            foreach (Product product in visibleProductsInCategory)
            {
                if (product.Manufacturer is null || manufacturersInProducts.Any(x => x.Id == product.Manufacturer.Id)) continue;

                manufacturersInProducts.Add(product.Manufacturer);
            }

            ManufacturersInSelect = manufacturersInProducts;
        }
        else
        {
            ManufacturersInSelect = await _manufacturerService.GetAllAsync();
        }

        IEnumerable<ManufacturerSearchResponse> manufacturerSearchResponses = ManufacturersInSelect
            .Select(x => new ManufacturerSearchResponse()
            {
                id = x.Id,
                displayName = x.RealCompanyName,
            });

        JsonResult result = new(manufacturerSearchResponses)
        {
            StatusCode = 200
        };

        return result;
    }

    public async Task<ViewComponentResult> OnPostSearchProductsAsync([FromBody] ProductSearchOptions productSearchOptions)
    {
        List<Product> productsToAdd = await SearchProductsAsync(productSearchOptions);

        return ViewComponent(
            ProductListItemsViewComponent.ComponentName,
            new
            {
                products = productsToAdd,
                currency = Currency,
                secondaryCurrency = SecondaryCurrency,
            });
    }

    private async Task<List<Product>> SearchProductsAsync(ProductSearchOptions productSearchOptions)
    {
        ProductSearchRequest productSearchRequest = GetSearchRequestFromInput(productSearchOptions);

        List<Product> products = await _productSearchService.SearchProductsAsync(productSearchRequest);

        List<Product>? productsToAdd = null;

        if (productSearchOptions.SortingMethod != null)
        {
            products = await SortProductsAsync(products, productSearchOptions.SortingMethod.Value);
        }
        else if (productSearchRequest.CategoryId == null
            && productSearchRequest.ManufacturerId == null
            && string.IsNullOrWhiteSpace(productSearchRequest.UserInputString))
        {
            SortProductsFromSecondCategoryUp(products);
        }

        int currentPageNumber = (productSearchOptions.PageNumber - 1) ?? 0;

        int productsToDisplayStartIndex = currentPageNumber * _searchResultsPageCount;

        int productsToDisplayCount = Math.Min(_searchResultsPageCount, products.Count - productsToDisplayStartIndex);

        productsToAdd ??= products.Slice(productsToDisplayStartIndex, productsToDisplayCount);

        return productsToAdd;
    }

    public async Task<ViewComponentResult> OnGetGetProductDataAsync([FromQuery] int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        ProductDataViewComponent.ProductDataExistingData productDataExistingData = new()
        {
            Product = product,
        };

        return ViewComponent(ProductDataComponentPath, new
        {
            existingData = productDataExistingData,
            dialogId = _dialogId,
        });
    }

    private static void SortProductsFromSecondCategoryUp(List<Product> products)
    {
        products.Sort(CompareProductsFromSecondCategoryUp);
    }

    private static int CompareProductsFromSecondCategoryUp(Product product1, Product product2)
    {
        int categorySortIndex1 = (product1.CategoryId > 1) ? product1.CategoryId.Value : int.MaxValue;
        int categorySortIndex2 = (product2.CategoryId > 1) ? product2.CategoryId.Value : int.MaxValue;

        int categoryComparison = categorySortIndex1 - categorySortIndex2;

        if (categoryComparison != 0) return categoryComparison;

        int displayOrder1 = product1.DisplayOrder ?? int.MaxValue;
        int displayOrder2 = product2.DisplayOrder ?? int.MaxValue;

        int displayOrderComparison = displayOrder1 - displayOrder2;

        if (displayOrderComparison != 0) return displayOrderComparison;

        return product1.Id.CompareTo(product2.Id);
    }

    private async Task<List<Product>> SortProductsAsync(List<Product> products, ProductSearchSortingOptions productSearchSortingOptions)
    {
        if (products.Count == 0 || productSearchSortingOptions == ProductSearchSortingOptions.Default) return products;

        if (productSearchSortingOptions == ProductSearchSortingOptions.PriceDescending)
        {
            return products.OrderBy(x => x.Price ?? int.MaxValue).ToList();
        }

        if (productSearchSortingOptions == ProductSearchSortingOptions.PriceAcsending)
        {
            return products.OrderByDescending(x => x.Price ?? int.MinValue).ToList();
        }

        if (productSearchSortingOptions == ProductSearchSortingOptions.PromotionsFirst)
        {
            IEnumerable<int> productIds = products.Select(x => x.Id);

            List<IGrouping<int?, Promotion>> promotionsForProducts = await _promotionService.GetAllForSelectionOfProductsAsync(productIds);

            Dictionary<int, List<Promotion>> promotionsByProductId = promotionsForProducts
                .Where(group => group.Key.HasValue)
                .ToDictionary(group => group.Key!.Value, group => group.ToList());

           return products
                .OrderBy(x => !DoesProductHaveAnyPromotions(promotionsByProductId, x))
                .ThenBy(x => x.DisplayOrder ?? int.MaxValue)
                .ToList();
        }

        return products
            .OrderBy(x => string.IsNullOrWhiteSpace(x.Name))
            .ThenBy(x => x.Name)
            .ThenBy(x => x.DisplayOrder)
            .ToList();
    }

    private static bool DoesProductHaveAnyPromotions(Dictionary<int, List<Promotion>> promotionsByProductId, Product product)
    {
        if (DoesProductHaveInfoPromotion(product))
        {
            return true;
        }

        if (!promotionsByProductId.TryGetValue(product.Id, out List<Promotion>? promotions)) return false;

        foreach (Promotion? promotion in promotions)
        {
            if ((promotion.Id == product.PromotionPid || promotion.Id == product.PromotionRid)
                && IsPromotionActive(promotion))
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesProductHaveInfoPromotion(Product product)
    {
        if (product.AlertExpireDate is not null && product.AlertExpireDate < DateTime.Now) return false;

        int? promotionPictureId = product.AlertPictureId;

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            promotionPictureId = product.PromotionPictureId;
        }

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            return false;
        }

        return true;
    }

    private static bool IsPromotionActive(Promotion promotion)
    {
        return (promotion.StartDate is null || promotion.StartDate <= DateTime.Now)
            && (promotion.ExpirationDate is null || promotion.ExpirationDate >= DateTime.Now);
    }

    private static ProductSearchRequest GetSearchRequestFromInput(ProductSearchOptions productSearchOptions)
    {
        return new ProductSearchRequest
        {
            OnlyVisibleByEndUsers = true,
            UserInputString = productSearchOptions.SearchData,
            CategoryId = productSearchOptions.CategoryId,
            ManufacturerId = productSearchOptions.ManufacturerId,
            ProductStatus = productSearchOptions.AvailableOnly == true ? ProductStatusSearchOptions.Available : ProductStatusSearchOptions.AvailableAndCall,
        };
    }
}