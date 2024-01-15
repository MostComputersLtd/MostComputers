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

    public static string[]? GetSearchStringParts(Product product)
    {
        if (product.SearchString == null) return null;

        string[] output = product.SearchString.Split(' ');

        return output;
    }

    public Dictionary<string, List<SearchStringPartOriginData>?>? GetSearchStringPartsAndDataAboutTheirOrigin(Product product)
    {
        if (product.SearchString == null) return null;

        Dictionary<string, List<SearchStringPartOriginData>?> output = new();

        string[] searchStringParts = product.SearchString
            .Trim()
            .Split(' ');

        List<ProductCharacteristic> categories = GetCharacteristicsRelatedToProduct(product);

        foreach (string searchStringPart in searchStringParts)
        {
            List<ProductCharacteristic>? productCharacteristics
             = GetCharacteristicsForSearchStringPart(categories, searchStringPart);

            if (productCharacteristics is null)
            {
                output.Add(searchStringPart, null);

                continue;
            }

            IEnumerable<SearchStringPartOriginData> productOriginDataFromRelatedCharacteristics
                = productCharacteristics.Select(characteristic => new SearchStringPartOriginData(searchStringPart, characteristic));

            output.Add(searchStringPart, productOriginDataFromRelatedCharacteristics.ToList());
        }

        return output;
    }

    private List<ProductCharacteristic> GetCharacteristicsRelatedToProduct(Product product)
    {
        List<int> categoriesToSearchIds = new() { -1 };

        if (product.CategoryID is not null)
        {
            categoriesToSearchIds.Add((int)product.CategoryID);
        }

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsForProductCategoriesInGroups
            = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoriesToSearchIds);

        List<ProductCharacteristic> characteristicsForProductCategories = new();

        foreach (IGrouping<int, ProductCharacteristic> characteristicsForGivenCategories in characteristicsForProductCategoriesInGroups)
        {
            characteristicsForProductCategories.AddRange(characteristicsForGivenCategories);
        }

        return characteristicsForProductCategories;
    }

    public static List<ProductCharacteristic>? GetCharacteristicsForSearchStringPart(
        IEnumerable<ProductCharacteristic> characteristicsAndSearchStringAbbreviationsForProduct,
        string searchStringPart)
    {
        ProductCharacteristic? output = characteristicsAndSearchStringAbbreviationsForProduct
             .FirstOrDefault(x => x.Name == searchStringPart);

        if (output is not null) return new() { output };

        output = characteristicsAndSearchStringAbbreviationsForProduct
            .FirstOrDefault(x => string.Equals(x.Name, searchStringPart, StringComparison.OrdinalIgnoreCase));

        if (output is not null) return new() { output };

        IEnumerable<ProductCharacteristic> characteristicsContainingInput = characteristicsAndSearchStringAbbreviationsForProduct
            .Where(x => x.Name?.Contains(searchStringPart) ?? false);

        if (characteristicsContainingInput.Any())
        {
            return characteristicsContainingInput.ToList();
        }

        return null;
    }
}