using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IProductTableDataService
{
    IReadOnlyList<ProductDisplayData> ProductData { get; }

    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(IEnumerable<ProductDisplayData> products);
    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(Func<IEnumerable<ProductDisplayData>> productFunc);
    Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync);
    void AddProductData(ProductDisplayData product, bool makeNewProductFirst = false);
    void Populate(IEnumerable<ProductDisplayData> products);
    bool RemoveProductByIndex(int index);
    bool RemoveProductById(int id);
    ProductDisplayData? GetProductById(int productId);
    bool AddProductDataIfProductDataWithSameIdDoesntExist(ProductDisplayData product, bool makeNewProductFirst = false);
}