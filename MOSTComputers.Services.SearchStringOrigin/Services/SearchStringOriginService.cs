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

    //public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin(Product product)
    //{
    //    if (product.SearchString == null) return null;

    //    List<Tuple<string, List<SearchStringPartOriginData>?>> output = new();

    //    string[] searchStringParts = product.SearchString
    //        .Trim()
    //        .Split(' ');

    //    List<ProductCharacteristic> characteristicsRelatedToProduct = GetCharacteristicsRelatedToProduct(product);

    //    for (int i = searchStringParts.Length - 1; i >= 0; i--)
    //    {
    //        string searchStringPart = searchStringParts[i];

    //        Tuple<string, List<SearchStringPartOriginData>?>? tupleWithSameValue = output.FirstOrDefault(x => x.Item1 == searchStringPart);

    //        if (tupleWithSameValue is not null)
    //        {
    //            output.Add(tupleWithSameValue);

    //            continue;
    //        }

    //        List<ProductCharacteristic>? productCharacteristics
    //            = GetCharacteristicsForSearchStringPart(characteristicsRelatedToProduct, searchStringPart);

    //        if (productCharacteristics is null)
    //        {
    //            output.Add(new(searchStringPart, null));

    //            continue;
    //        }

    //        //bool doesPartHaveAnExactCharacteristic = DoesPartHaveAnExactCharacteristic(searchStringPart, productCharacteristics);

    //        //if (!doesPartHaveAnExactCharacteristic)
    //        //{
    //        //    Tuple<string, List<SearchStringPartOriginData>?> tupleOfPartAfterThisPart = output.Last();

    //        //    List<SearchStringPartOriginData>? partAfterThisOneOriginData = tupleOfPartAfterThisPart.Item2;

    //        //    bool doesPartAfterThisOneHaveAnExactOriginData = DoesPartHaveAnExactOriginData(tupleOfPartAfterThisPart.Item1, partAfterThisOneOriginData);

    //        //    if (!doesPartAfterThisOneHaveAnExactOriginData)
    //        //    {
    //        //        for (int j = 0; j < productCharacteristics.Count; j++)
    //        //        {
    //        //            ProductCharacteristic notExactCharacteristic = productCharacteristics[j];

    //        //            if (notExactCharacteristic.Name is null) continue;

    //        //            string[] characteristicNameSplitParts = notExactCharacteristic.Name.Split(' ');

    //        //            for (int k = 0; k < characteristicNameSplitParts.Length; k++)
    //        //            {
    //        //                string characteristicNameParts = characteristicNameSplitParts[k];

    //        //                var outputEntryFromCharacteristicPart = 
    //        //            }
    //        //        }
    //        //    }
    //        //}

    //        IEnumerable<SearchStringPartOriginData> productOriginDataFromRelatedCharacteristics
    //            = productCharacteristics.Select(characteristic => new SearchStringPartOriginData(searchStringPart, characteristic));

    //        output.Add(new(searchStringPart, productOriginDataFromRelatedCharacteristics.ToList()));
    //    }

    //    output.Reverse();

    //    return output;
    //}

    public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin(Product product)
    {
        if (product.SearchString == null) return null;

        List<string>? searchStringPartsSimple = GetSearchStringPartsSimple(product);

        if (searchStringPartsSimple == null) return null;

        List<Tuple<string, List<SearchStringPartOriginData>?>> output = new();

        List<ProductCharacteristic> characteristicsRelatedToProduct = GetCharacteristicsRelatedToProduct(product);

        List<string[]> searchStringCompositePartRegions = new();

        int currentRegionStartIndex = 0;

        for (int i = 0; i < searchStringPartsSimple.Count - 1; i++)
        {
            string searchStringSinglePart = searchStringPartsSimple[i];

            ProductCharacteristic? productCharacteristicExactMatch
                = characteristicsRelatedToProduct.FirstOrDefault(characteristic
                    => string.Equals(characteristic.Name, searchStringSinglePart, StringComparison.OrdinalIgnoreCase));

            if (productCharacteristicExactMatch != null)
            {
                SearchStringPartOriginData searchStringPartOriginDataForSimple = new(searchStringSinglePart, productCharacteristicExactMatch);

                output.Add(new(searchStringSinglePart, new() { searchStringPartOriginDataForSimple }));

                if (currentRegionStartIndex != i)
                {
                    string[] regionOfUnmatchedItems = searchStringPartsSimple
                        .Skip(currentRegionStartIndex)
                        .Take(i - currentRegionStartIndex)
                        .ToArray();

                    searchStringCompositePartRegions.Add(regionOfUnmatchedItems);
                }

                currentRegionStartIndex = i + 1;
            }
        }

        int simplePartsLastItemIndex = searchStringPartsSimple.Count - 1;

        string searchStringLastSinglePart = searchStringPartsSimple[simplePartsLastItemIndex];

        ProductCharacteristic? lastProductCharacteristicExactMatch
            = characteristicsRelatedToProduct.FirstOrDefault(characteristic
                => string.Equals(characteristic.Name, searchStringLastSinglePart, StringComparison.OrdinalIgnoreCase));

        if (lastProductCharacteristicExactMatch != null)
        {
            SearchStringPartOriginData searchStringPartOriginDataForLastSimple = new(searchStringLastSinglePart, lastProductCharacteristicExactMatch);

            output.Add(new(searchStringLastSinglePart, new() { searchStringPartOriginDataForLastSimple }));
        }

        if (currentRegionStartIndex != simplePartsLastItemIndex)
        {
            if (lastProductCharacteristicExactMatch is null)
            {
                simplePartsLastItemIndex++;
            }

            string[] regionOfUnmatchedItems = searchStringPartsSimple
                .Skip(currentRegionStartIndex)
                .Take(simplePartsLastItemIndex - currentRegionStartIndex)
                .ToArray();

            searchStringCompositePartRegions.Add(regionOfUnmatchedItems);
        }

        for (int i = 0; i < searchStringCompositePartRegions.Count; i++)
        {
            string[] compositePartsRegion = searchStringCompositePartRegions[i];

            if (compositePartsRegion.Length == 1)
            {
                string part = compositePartsRegion[0];

                List<SearchStringPartOriginData> characteristicsThatContainTheSearchedWordOriginData = characteristicsRelatedToProduct
                    .Where(characteristic => characteristic.Name?.Contains(part, StringComparison.OrdinalIgnoreCase) ?? false)
                    .Select(characteristic => new SearchStringPartOriginData(part, characteristic))
                    .ToList();

                output.Add(new(part, characteristicsThatContainTheSearchedWordOriginData));

                continue;
            }

            bool lastItemInPartsIsMatched = false;

            for (int j = 0; j < compositePartsRegion.Length - 1; j++)
            {
                string part = compositePartsRegion[j];

                string matchString = part;

                bool foundCompositeMatch = false;

                IEnumerable<ProductCharacteristic> characteristicsThatContainTheSearchedWord = characteristicsRelatedToProduct
                    .Where(characteristic => characteristic.Name?.Contains(part, StringComparison.OrdinalIgnoreCase) ?? false);

                for (int k = j + 1; k < compositePartsRegion.Length; k++)
                {
                    string afterPart = compositePartsRegion[k];

                    matchString += (" " + afterPart);

                    ProductCharacteristic? compositeExactMatch = characteristicsThatContainTheSearchedWord.FirstOrDefault(characteristic
                        => string.Equals(characteristic.Name, matchString, StringComparison.OrdinalIgnoreCase));

                    if (compositeExactMatch is not null)
                    {
                        if (k == compositePartsRegion.Length - 1)
                        {
                            lastItemInPartsIsMatched = true;
                        }

                        SearchStringPartOriginData compositeMatchOriginData = new(matchString, compositeExactMatch);

                        output.Add(new(matchString, new List<SearchStringPartOriginData>() { compositeMatchOriginData }));

                        compositePartsRegion = compositePartsRegion
                            .Skip(k)
                            .ToArray();

                        j = k + 1;

                        foundCompositeMatch = true;

                        break;
                    }
                }

                if (!foundCompositeMatch)
                {
                    List<SearchStringPartOriginData> multipleMatchOriginList
                        = characteristicsThatContainTheSearchedWord.Select(characteristic => new SearchStringPartOriginData(part, characteristic))
                            .ToList();

                    output.Add(new(part, multipleMatchOriginList));
                }
            }

            if (!lastItemInPartsIsMatched)
            {
                string compositePartsLastRegion = compositePartsRegion[^1];

                List<SearchStringPartOriginData> characteristicsThatContainTheSearchedWord = characteristicsRelatedToProduct
                    .Where(characteristic => characteristic.Name?.Contains(compositePartsLastRegion, StringComparison.OrdinalIgnoreCase) ?? false)
                    .Select(characteristic => new SearchStringPartOriginData(compositePartsLastRegion, characteristic))
                    .ToList();

                output.Add(new(compositePartsLastRegion, characteristicsThatContainTheSearchedWord));
            }
        }

        return output
            .OrderBy(tuple => product.SearchString.IndexOf(tuple.Item1))
            .ToList();
    }

    private static List<string>? GetSearchStringPartsSimple(Product product)
    {
        if (product.SearchString == null) return null;

        List<string> searchStringPartsSimple = product.SearchString
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

    private List<ProductCharacteristic> GetCharacteristicsRelatedToProduct(Product product)
    {
        List<int> categoriesToSearchIds = new() { -1 };

        if (product.CategoryId is not null)
        {
            categoriesToSearchIds.Add((int)product.CategoryId);
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