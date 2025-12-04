using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;

namespace MOSTComputers.Services.SearchStringOrigin.Services;
internal class SearchStringOriginService : ISearchStringOriginService
{
    public SearchStringOriginService(
        IProductCharacteristicService productCharacteristicService)
    {
        _productCharacteristicService = productCharacteristicService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;

    public async Task<Dictionary<int, List<SearchStringPartOriginData>>> GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(IEnumerable<Product> products)
    {
        IEnumerable<int> categoryIds = products
            .Select(x => x.CategoryId)
            .Distinct()
            .Where(x => x is not null)
            .Cast<int>();

        List<ProductCharacteristic> characteristicsRelatedToProducts = await GetSearchStringAbbreviationsRelatedToProductsAsync(categoryIds);

        Dictionary<int, List<SearchStringPartOriginData>> output = new();

        foreach (Product product in products)
        {
            if (product.SearchString is null) continue;

            List<string>? searchStringPartsSimple = GetSearchStringPartsSimple(product.SearchString);

            if (searchStringPartsSimple is null) continue;

            List<SearchStringPartOriginData> searchStringPartsForProduct = new();

            foreach (string searchStringPart in searchStringPartsSimple)
            {
                ProductCharacteristic? productCharacteristic = characteristicsRelatedToProducts
                    .FirstOrDefault(x => x.Name == searchStringPart);

                if (productCharacteristic is null) continue;

                SearchStringPartOriginData searchStringPartOriginData = new(searchStringPart, productCharacteristic);

                searchStringPartsForProduct.Add(searchStringPartOriginData);
            }

            output.Add(product.Id, searchStringPartsForProduct);
        }

        return output;
    }

    public async Task<List<SearchStringPartOriginData>?> GetSearchStringPartsAndDataAboutTheirOriginAsync(string? searchString, int? categoryId)
    {
        if (searchString is null) return null;

        List<string>? searchStringPartsSimple = GetSearchStringPartsSimple(searchString);

        if (searchStringPartsSimple is null) return null;

        List<SearchStringPartOriginData> output = new();

        List<ProductCharacteristic> characteristicsRelatedToProduct = await GetSearchStringAbbreviationsRelatedToProductAsync(categoryId);

        foreach (string searchStringPart in searchStringPartsSimple)
        {
            ProductCharacteristic? productCharacteristic = characteristicsRelatedToProduct
                .FirstOrDefault(x => x.Name == searchStringPart);

            if (productCharacteristic is null) continue;

            SearchStringPartOriginData searchStringPartOriginData = new(searchStringPart, productCharacteristic);

            output.Add(searchStringPartOriginData);
        }

        return output.OrderBy(x => x.Characteristic.DisplayOrder ?? int.MaxValue)
            .ToList();
    }

    private static List<string>? GetSearchStringPartsSimple(string? searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString)) return null;

        List<string> searchStringPartsSimple = searchString
            .Trim()
            .Split(' ')
            .ToList();

        for (int i = 0; i < searchStringPartsSimple.Count; i++)
        {
            string searchStringPart = searchStringPartsSimple[i];

            if (string.IsNullOrWhiteSpace(searchStringPart))
            {
                searchStringPartsSimple.RemoveAt(i);

                i--;
            }
        }

        return searchStringPartsSimple;
    }

    private async Task<List<ProductCharacteristic>> GetSearchStringAbbreviationsRelatedToProductsAsync(IEnumerable<int>? categoryIds)
    {
        List<int> categoriesToSearchIds = new() { -1 };

        if (categoryIds is not null)
        {
            categoriesToSearchIds.AddRange(categoryIds);
        }

        return await GetSearchStringAbbreviationsForCategoriesAsync(categoriesToSearchIds);
    }

    private async Task<List<ProductCharacteristic>> GetSearchStringAbbreviationsRelatedToProductAsync(int? categoryId)
    {
        List<int> categoriesToSearchIds = new() { -1 };

        if (categoryId is not null)
        {
            categoriesToSearchIds.Add(categoryId.Value);
        }

        return await GetSearchStringAbbreviationsForCategoriesAsync(categoriesToSearchIds);
    }

    private async Task<List<ProductCharacteristic>> GetSearchStringAbbreviationsForCategoriesAsync(List<int> categoriesToSearchIds)
    {
        List<ProductCharacteristic> characteristicsForProductCategories
            = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(categoriesToSearchIds, [ProductCharacteristicType.SearchStringAbbreviation], true);

        return characteristicsForProductCategories;
    }
}