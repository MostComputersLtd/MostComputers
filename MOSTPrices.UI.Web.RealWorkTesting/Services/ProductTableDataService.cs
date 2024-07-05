using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class ProductTableDataService : IProductTableDataService
{
    private List<ProductDisplayData> _products = new();

    public IReadOnlyList<ProductDisplayData> ProductData => _products;

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(IEnumerable<ProductDisplayData> products)
    {
        if (_products.Count <= 0)
        {
            _products = products.ToList();
        }

        return _products;
    }

    public IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(Func<IEnumerable<ProductDisplayData>> productFunc)
    {
        if (_products.Count <= 0)
        {
            _products = productFunc().ToList();
        }

        return _products;
    }

    public async Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync)
    {
        if (_products.Count <= 0)
        {
            IEnumerable<ProductDisplayData> enumerable = await productFuncAsync();

            _products = enumerable.ToList();
        }

        return _products;
    }

    public ProductDisplayData? GetProductById(int productId)
    {
        return _products.FirstOrDefault(product => product.Id == productId);
    }

    public void Populate(IEnumerable<ProductDisplayData> products)
    {
        _products = products.ToList();
    }

    public void AddProductData(ProductDisplayData product, bool makeNewProductFirst = false)
    {
        if (makeNewProductFirst)
        {
            _products.Insert(0, product);

            return;
        }

        _products.Add(product);
    }

    public bool AddProductDataIfProductDataWithSameIdDoesntExist(ProductDisplayData product, bool makeNewProductFirst = false)
    {
        if (_products.FirstOrDefault(x => x.Id == product.Id) != null)
        {
            return false;
        }

        if (makeNewProductFirst)
        {
            _products.Insert(0, product);

            return true;
        }

        _products.Add(product);

        return true;
    }

    public bool RemoveProductByIndex(int index)
    {
        if (index < 0 || index >= _products.Count)
        {
            return false;
        }

        _products.RemoveAt(index);

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

        return true;
    }
}