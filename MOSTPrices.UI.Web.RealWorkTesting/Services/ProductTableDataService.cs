using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class ProductTableDataService : IProductTableDataService
{
    private List<ProductDisplayData> _products = new();
    private readonly List<int> _idsOfProductsToDisplay = new();

    public IReadOnlyList<ProductDisplayData> ProductData => _products;

    public IReadOnlyList<ProductDisplayData> GetProductDataToDisplay()
    {
        return _products
        .Where(product => _idsOfProductsToDisplay.Contains(product.Id))
        .ToList();
    }

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(IEnumerable<ProductDisplayData> products, bool areNewProductsDisplayable = true)
    {
        if (_products.Count <= 0)
        {
            _products = products.ToList();

            if (!areNewProductsDisplayable) return _products;

            foreach (ProductDisplayData product in _products)
            {
                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(
        IEnumerable<ProductDisplayData> products,
        Func<ProductDisplayData, bool> areNewProductsDisplayableFunc)
    {
        if (_products.Count <= 0)
        {
            _products = products.ToList();

            foreach (ProductDisplayData product in _products)
            {
                if (!areNewProductsDisplayableFunc(product)) continue;

                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(Func<IEnumerable<ProductDisplayData>> productFunc, bool areNewProductsDisplayable)
    {
        if (_products.Count <= 0)
        {
            _products = productFunc().ToList();

            if (!areNewProductsDisplayable) return _products;

            foreach (ProductDisplayData product in _products)
            {
                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(
        Func<IEnumerable<ProductDisplayData>> productFunc,
        Func<ProductDisplayData, bool> areNewProductsDisplayableFunc)
    {
        if (_products.Count <= 0)
        {
            _products = productFunc().ToList();

            foreach (ProductDisplayData product in _products)
            {
                if (!areNewProductsDisplayableFunc(product)) continue;

                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public async Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(
        Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync,
        bool areNewProductsDisplayable = true)
    {
        if (_products.Count <= 0)
        {
            IEnumerable<ProductDisplayData> enumerable = await productFuncAsync();

            _products = enumerable.ToList();

            if (!areNewProductsDisplayable) return _products;

            foreach (ProductDisplayData product in _products)
            {
                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public async Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(
        Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync,
        Func<ProductDisplayData, bool> areNewProductsDisplayableFunc)
    {
        if (_products.Count <= 0)
        {
            IEnumerable<ProductDisplayData> enumerable = await productFuncAsync();

            _products = enumerable.ToList();

            foreach (ProductDisplayData product in _products)
            {
                if (!areNewProductsDisplayableFunc(product)) continue;

                _idsOfProductsToDisplay.Add(product.Id);
            }
        }

        return _products;
    }

    public void Populate(IEnumerable<ProductDisplayData> products, bool areNewProductsDisplayable = true)
    {
        _products = products.ToList();

        if (!areNewProductsDisplayable) return;

        foreach (ProductDisplayData product in _products)
        {
            _idsOfProductsToDisplay.Add(product.Id);
        }
    }

    public void Populate(IEnumerable<ProductDisplayData> products, Func<ProductDisplayData, bool> areNewProductsDisplayableFunc)
    {
        _products = products.ToList();

        foreach (ProductDisplayData product in _products)
        {
            if (!areNewProductsDisplayableFunc(product)) continue;

            _idsOfProductsToDisplay.Add(product.Id);
        }
    }

    public bool AddToDisplayableProductIdsIfIdIsntAlreadyAdded(int productId)
    {
        if (_idsOfProductsToDisplay.Contains(productId)) return false;

        _idsOfProductsToDisplay.Add(productId);

        return true;
    }

    public void RemoveProductIdFromIdsToDisplay(int productId)
    {
        int productIdIndex = _idsOfProductsToDisplay.FindIndex(x => x == productId);

        if (productIdIndex < 0) return;

        _idsOfProductsToDisplay.RemoveAt(productIdIndex);
    }

    public void RemoveAllIdsFromIdsToDisplay()
    {
        _idsOfProductsToDisplay.Clear();
    }

    public ProductDisplayData? GetProductById(int productId)
    {
        return _products.FirstOrDefault(product => product.Id == productId);
    }

    public bool AddProductDataOrMakeExistingOneDisplayable(ProductDisplayData product, bool makeNewProductFirst = false, bool isNewOrOldProductDisplayable = true)
    {
        if (_products.FirstOrDefault(x => x.Id == product.Id) != null)
        {
            if (isNewOrOldProductDisplayable)
            {
                AddToDisplayableProductIdsIfIdIsntAlreadyAdded(product.Id);
            }

            return false;
        }

        if (makeNewProductFirst)
        {
            _products.Insert(0, product);

            if (isNewOrOldProductDisplayable)
            {
                AddToDisplayableProductIdsIfIdIsntAlreadyAdded(product.Id);
            }

            return true;
        }

        _products.Add(product);

        if (isNewOrOldProductDisplayable)
        {
            AddToDisplayableProductIdsIfIdIsntAlreadyAdded(product.Id);
        }

        return true;
    }

    public void AddOrUpdateProductData(ProductDisplayData product, bool makeProductFirst = false, bool isNewProductDisplayable = true)
    {
        int indexOfProductData = _products.FindIndex(x => x.Id == product.Id);

        if (indexOfProductData >= 0)
        {
            _products[indexOfProductData] = product;

            return;
        }

        if (makeProductFirst)
        {
            _products.Insert(0, product);

            if (isNewProductDisplayable)
            {
                AddToDisplayableProductIdsIfIdIsntAlreadyAdded(product.Id);
            }

            return;
        }

        _products.Add(product);

        if (isNewProductDisplayable)
        {
            AddToDisplayableProductIdsIfIdIsntAlreadyAdded(product.Id);
        }

        return;
    }

    public bool RemoveProductByIndex(int index)
    {
        if (index < 0 || index >= _products.Count)
        {
            return false;
        }

        int productId = _products[index].Id;

        _products.RemoveAt(index);

        RemoveProductIdFromIdsToDisplay(productId);

        return true;
    }

    public bool RemoveProductById(int productId)
    {
        if (productId <= 0)
        {
            return false;
        }

        int indexOfItemToRemove = _products.FindIndex(x => x.Id == productId);

        if (indexOfItemToRemove < 0) return false;

        _products.RemoveAt(indexOfItemToRemove);

        RemoveProductIdFromIdsToDisplay(productId);

        return true;
    }
}