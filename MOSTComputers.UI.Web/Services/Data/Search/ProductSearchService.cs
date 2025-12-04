using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.UI.Web.Models.ProductSearch;
using MOSTComputers.UI.Web.Services.Data.Search.Contracts;

namespace MOSTComputers.UI.Web.Services.Data.Search;

internal sealed class ProductSearchService : IProductSearchService
{
    public ProductSearchService(
        IProductService productService,
        IProductWorkStatusesService productWorkStatusesService,
        IPromotionService promotionService,
        IOriginalLocalChangesReadService originalLocalChangesReadService)
    {
        _productService = productService;
        _productWorkStatusesService = productWorkStatusesService;
        _promotionService = promotionService;
        _originalLocalChangesReadService = originalLocalChangesReadService;
    }

    private const char _searchesSplitCharacter = ',';

    private const char _searchPartsSplitCharacter = ' ';

    private const char _optionListStartCharacter = '[';
    private const char _optionListEndCharacter = ']';

    private static readonly char[] _charsToSplitSearchPartsIntoOptionsBy = new char[] { ',', ' ' };

    private readonly IProductService _productService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IPromotionService _promotionService;
    private readonly IOriginalLocalChangesReadService _originalLocalChangesReadService;

    public async Task<List<Product>> SearchProductsAsync(ProductSearchRequest productSearchRequest)
    {
        List<Product> products = await _productService.GetAllAsync();

        return await SearchProductsAsync(products, productSearchRequest);
    }

    public async Task<List<Product>> SearchProductsAsync(IEnumerable<Product> productsToSearchIn, ProductSearchRequest productSearchRequest)
    {
        List<Product>? searchedProducts = new(productsToSearchIn);

        if (!string.IsNullOrWhiteSpace(productSearchRequest.UserInputString))
        {
            searchedProducts = GetProductsFromSearchData(productsToSearchIn, productSearchRequest.UserInputString);
        }

        if (productSearchRequest.CategoryId is not null)
        {
            searchedProducts = searchedProducts
                .Where(x => x.CategoryId == productSearchRequest.CategoryId)
                .ToList();
        }

        if (productSearchRequest.ProductStatus is not null)
        {
            searchedProducts = GetFilteredProductsByStatus(productSearchRequest.ProductStatus.Value, searchedProducts);
        }

        if (productSearchRequest.ProductNewStatuses is not null)
        {
            searchedProducts = await GetFilteredProductsByNewStatusAsync(productSearchRequest.ProductNewStatuses, searchedProducts);
        }

        if (productSearchRequest.PromotionSearchOptions is not null)
        {
            searchedProducts = await GetFilteredProductsByPromotionSearchEnumAsync(productSearchRequest.PromotionSearchOptions.Value, searchedProducts);
        }

        if (productSearchRequest.MaxResultCount is not null
            && productSearchRequest.MaxResultCount > 0)
        {
            searchedProducts = searchedProducts
                .Take(productSearchRequest.MaxResultCount.Value)
                .ToList();
        }

        return searchedProducts;
    }

    private static List<Product> GetProductsFromSearchData(IEnumerable<Product> productsToSearchIn, string searchData)
    {
        List<string> separateSearches = SplitSearchDataIntoSingleSearches(searchData);

        List<Product> output = new();

        foreach (string separateSearch in separateSearches)
        {
            List<string> searchParts = SplitSearchDataInParts(separateSearch);

            List<Product> productsFromSearch = GetProductsFromSearch(productsToSearchIn, searchParts);

            IEnumerable<Product> newProductsFromSearch = productsFromSearch
                .Where(newProduct =>
                {
                    Product? alreadyAddedProduct = output.FirstOrDefault(addedProduct => addedProduct.Id == newProduct.Id);

                    return alreadyAddedProduct is null;
                });

            output.AddRange(newProductsFromSearch);
        }

        return output;
    }

    private static List<Product> GetProductsFromSearch(IEnumerable<Product> allProducts, List<string> searchParts)
    {
        List<Product> output = new();

        foreach (Product product in allProducts)
        {
            bool shouldAddProduct = true;

            foreach (string searchPart in searchParts)
            {
                shouldAddProduct = DoesProductMatchSearchPartOrSearchOptions(product, searchPart);

                if (!shouldAddProduct) break;
            }

            if (!shouldAddProduct) continue;

            output.Add(product);
        }

        return output;
    }

    private static bool DoesProductMatchSearchPartOrSearchOptions(Product product, string searchPart)
    {
        if (!searchPart.StartsWith(_optionListStartCharacter)
            || !searchPart.EndsWith(_optionListEndCharacter))
        {
            return DoesProductMatchSearchPart(product, searchPart);
        }

        List<string> searchOptions = SplitSearchKeywordIntoOptions(searchPart);

        foreach (string option in searchOptions)
        {
            bool doesMatchOption = DoesProductMatchSearchPart(product, option);

            if (doesMatchOption) return true;
        }

        return false;
    }

    private static bool DoesProductMatchSearchPart(Product product, string searchPart)
    {
        bool doesIdMatchSearchPart = product.Id.ToString().Equals(searchPart, StringComparison.OrdinalIgnoreCase);

        if (doesIdMatchSearchPart) return true;

        bool doesNameMatchSearchPart = product.Name?.Contains(searchPart, StringComparison.OrdinalIgnoreCase) ?? false;

        if (doesNameMatchSearchPart) return true;

        bool doesPartNumber1MatchSearchPart = product.PartNumber1?.Contains(searchPart, StringComparison.OrdinalIgnoreCase) ?? false;

        if (doesPartNumber1MatchSearchPart) return true;

        bool doesPartNumber2MatchSearchPart = product.PartNumber2?.Contains(searchPart, StringComparison.OrdinalIgnoreCase) ?? false;

        if (doesPartNumber2MatchSearchPart) return true;

        bool doesSearchStringMatchSearchPart = product.SearchString?.Contains(searchPart, StringComparison.OrdinalIgnoreCase) ?? false;

        return doesSearchStringMatchSearchPart;
    }

    private static List<string> SplitSearchDataIntoSingleSearches(string searchData)
    {
        List<string> searches = new();

        int start = 0;

        bool insideOptionList = false;

        for (int i = 0; i < searchData.Length; i++)
        {
            if (searchData[i] == _optionListStartCharacter)
            {
                insideOptionList = true;
            }
            else if (searchData[i] == _optionListEndCharacter)
            {
                insideOptionList = false;
            }
            else if (searchData[i] == _searchesSplitCharacter && !insideOptionList)
            {
                string search = searchData[start..i].Trim();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    searches.Add(search);
                }

                start = i + 1;
            }
        }

        if (start < searchData.Length)
        {
            string lastSearch = searchData[start..].Trim();

            if (string.IsNullOrWhiteSpace(lastSearch)) return searches;

            searches.Add(lastSearch);
        }

        return searches;
    }

    private static List<string> SplitSearchDataInParts(string singleSearchData)
    {
        List<string> parts = new();

        int start = 0;

        while (start < singleSearchData.Length)
        {
            int end = singleSearchData.IndexOf(_searchPartsSplitCharacter, start);

            if (singleSearchData[start] == _optionListStartCharacter)
            {
                int endOfOptionsList = singleSearchData.IndexOf(_optionListEndCharacter, start);

                if (endOfOptionsList >= 0)
                {
                    end = endOfOptionsList + 1;
                }
            }

            if (end <= 0)
            {
                parts.Add(singleSearchData[start..]);

                break;
            }

            parts.Add(singleSearchData[start..end]);

            while (end < singleSearchData.Length
                && singleSearchData[end] == _searchPartsSplitCharacter)
            {
                end++;
            }

            start = end;
        }

        return parts;
    }

    private static List<string> SplitSearchKeywordIntoOptions(string searchPart)
    {
        List<string> options = new();

        if (!searchPart.StartsWith(_optionListStartCharacter)
            || !searchPart.EndsWith(_optionListEndCharacter))
        {
            options.Add(searchPart);

            return options;
        }

        searchPart = searchPart[1..^1];

        int start = 0;

        while (start < searchPart.Length)
        {
            int splitIndex = searchPart.IndexOfAny(_charsToSplitSearchPartsIntoOptionsBy, start);

            if (splitIndex == -1)
            {
                options.Add(searchPart[start..]);

                return options;
            }

            options.Add(searchPart[start..splitIndex]);

            splitIndex++;

            while (splitIndex < searchPart.Length)
            {
                bool isEqualToSplitCharacter = false;

                foreach (char charToSplitBy in _charsToSplitSearchPartsIntoOptionsBy)
                {
                    if (searchPart[splitIndex] != charToSplitBy) continue;

                    isEqualToSplitCharacter = true;

                    break;
                }

                if (!isEqualToSplitCharacter) break;

                splitIndex++;
            }

            start = splitIndex;
        }

        return options;
    }

    private static List<Product> GetFilteredProductsByStatus(ProductStatusSearchOptions productStatusSearchEnum, IEnumerable<Product> products)
    {
        List<Product> filteredProducts = new();

        if (productStatusSearchEnum == ProductStatusSearchOptions.AvailableAndCall)
        {
            foreach (Product product in products)
            {
                if (product.Status == ProductStatus.Unavailable) continue;

                filteredProducts.Add(product);
            }

            return filteredProducts;
        }

        ProductStatus? productStatusEnumToSearchFor = productStatusSearchEnum switch
        {
            ProductStatusSearchOptions.Unavailable => ProductStatus.Unavailable,
            ProductStatusSearchOptions.Call => ProductStatus.Call,
            ProductStatusSearchOptions.Available => ProductStatus.Available,
            _ => null
        };

        if (productStatusEnumToSearchFor is null) return products.ToList();

        foreach (Product product in products)
        {
            if (product.Status != productStatusEnumToSearchFor) continue;

            filteredProducts.Add(product);
        }

        return filteredProducts;
    }

    private async Task<List<Product>> GetFilteredProductsByNewStatusAsync(List<ProductNewStatusSearchOptions> productNewStatuses, IEnumerable<Product> products)
    {
        IEnumerable<int> productIds = products.Select(x => x.Id);

        List<ProductWorkStatuses> productWorkStatuses = await _productWorkStatusesService.GetAllForProductsAsync(productIds);

        List<Product> filteredProducts = await GetFilteredProductsByNewStatusAsync(products, productWorkStatuses, productNewStatuses);

        return filteredProducts;
    }

    private async Task<List<Product>> GetFilteredProductsByNewStatusAsync(
        IEnumerable<Product> products,
        IEnumerable<ProductWorkStatuses> productStatuses,
        List<ProductNewStatusSearchOptions> productNewStatuses)
    {
        List<Product> filteredProducts = new();

        bool filterByLastAdded = productNewStatuses.Contains(ProductNewStatusSearchOptions.LastAdded);

        if (filterByLastAdded)
        {
            productNewStatuses.RemoveAll(x => x == ProductNewStatusSearchOptions.LastAdded);
        }

        if (productNewStatuses.Count <= 0)
        {
            filteredProducts = products.ToList();
        }

        foreach (Product product in products)
        {
            ProductWorkStatuses? productWorkStatuses = productStatuses.FirstOrDefault(x => x.ProductId == product.Id);

            if (productWorkStatuses is null) continue;

            foreach (ProductNewStatusSearchOptions productNewSearchStatus in productNewStatuses)
            {
                if (!DoesProductNewSearchStatusMatch(productNewSearchStatus, productWorkStatuses.ProductNewStatus)) continue;

                filteredProducts.Add(product);

                break;
            }
        }

        if (filterByLastAdded)
        {
            List<LocalChangeData> localChangeDatas = await _originalLocalChangesReadService.GetAllForTableNameAndOperationTypeAsync(
                _originalLocalChangesReadService.ProductsTableName, ChangeOperationType.Create);

            filteredProducts = GetOrderedProductsByLastAddedDate(filteredProducts, localChangeDatas);
        }

        return filteredProducts;
    }

    private static bool DoesProductNewSearchStatusMatch(ProductNewStatusSearchOptions searchedStatus, ProductNewStatus? productNewStatus)
    {
        return searchedStatus switch
        {
            ProductNewStatusSearchOptions.New => productNewStatus == ProductNewStatus.New,
            ProductNewStatusSearchOptions.WorkInProgress => productNewStatus == ProductNewStatus.WorkInProgress,
            ProductNewStatusSearchOptions.ReadyForUse => productNewStatus == ProductNewStatus.ReadyForUse,
            ProductNewStatusSearchOptions.Postponed1 => productNewStatus == ProductNewStatus.Postponed1,
            ProductNewStatusSearchOptions.Postponed2 => productNewStatus == ProductNewStatus.Postponed2,
            ProductNewStatusSearchOptions.LastAdded => false,
            _ => false
        };
    }

    private List<Product> GetOrderedProductsByLastAddedDate(
        IEnumerable<Product> products,
        IEnumerable<LocalChangeData> localChangeDatas)
    {
        IEnumerable<LocalChangeData> productAddedChanges = localChangeDatas.Where(
            change => change.TableName == _originalLocalChangesReadService.ProductsTableName
                && change.OperationType == ChangeOperationType.Create);

        return products.OrderByDescending(
            product =>
            {
                LocalChangeData? changeForProduct = productAddedChanges.FirstOrDefault(x => x.TableElementId == product.Id);

                return changeForProduct?.TimeStamp ?? DateTime.MinValue;
            })
            .ToList();
    }

    private async Task<List<Product>> GetFilteredProductsByPromotionSearchEnumAsync(PromotionSearchOptions promotionSearchEnum, IEnumerable<Product> products)
    {
        List<Product> filteredProducts = new();

        if (promotionSearchEnum == PromotionSearchOptions.P)
        {
            foreach (Product product in products)
            {
                if (product.PromotionPid is null) continue;

                filteredProducts.Add(product);
            }

            return filteredProducts;
        }
        else if (promotionSearchEnum == PromotionSearchOptions.R)
        {
            foreach (Product product in products)
            {
                if (product.PromotionRid is null) continue;

                filteredProducts.Add(product);
            }

            return filteredProducts;
        }
        else if (promotionSearchEnum == PromotionSearchOptions.I)
        {
            foreach (Product product in products)
            {
                if (product.AlertPictureId is null
                    || product.AlertPictureId <= 0)
                {
                    continue;
                }

                filteredProducts.Add(product);
            }

            return filteredProducts;
        }
        else if (promotionSearchEnum == PromotionSearchOptions.None)
        {
            foreach (Product product in products)
            {
                if (product.PromotionPid is not null
                    || product.PromotionRid is not null
                    || product.AlertPictureId > 0)
                {
                    continue;
                }

                filteredProducts.Add(product);
            }

            return filteredProducts;
        }
        else if (promotionSearchEnum == PromotionSearchOptions.Discount)
        {
            IEnumerable<int> productIds = products.Select(x => x.Id);

            List<IGrouping<int?, Promotion>> promotionsForProducts = await _promotionService.GetAllActiveForSelectionOfProductsAsync(productIds);

            foreach (Product product in products)
            {
                IGrouping<int?, Promotion>? promotionsForProduct = promotionsForProducts.FirstOrDefault(x => x.Key == product.Id);

                if (promotionsForProduct is null) continue;

                foreach (Promotion promotion in promotionsForProduct)
                {
                    if (promotion.DiscountEUR > 0M
                        || promotion.DiscountUSD > 0M)
                    {
                        filteredProducts.Add(product);

                        break;
                    }
                }
            }

            return filteredProducts;
        }

        return filteredProducts;
    }
}