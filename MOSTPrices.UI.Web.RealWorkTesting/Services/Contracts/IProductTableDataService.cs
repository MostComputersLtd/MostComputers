using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IProductTableDataService
{
    IReadOnlyList<ProductDisplayData> ProductData { get; }

    ProductDisplayData? GetProductById(int productId);
    IReadOnlyList<ProductDisplayData> GetProductDataToDisplay();
    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(IEnumerable<ProductDisplayData> products, bool areNewProductsDisplayable = true);
    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(IEnumerable<ProductDisplayData> products, Func<ProductDisplayData, bool> areNewProductsDisplayableFunc);
    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(Func<IEnumerable<ProductDisplayData>> productFunc, bool areNewProductsDisplayable = true);
    IReadOnlyList<ProductDisplayData> PopulateIfEmptyAndGet(Func<IEnumerable<ProductDisplayData>> productFunc, Func<ProductDisplayData, bool> areNewProductsDisplayableFunc);
    Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync, bool areNewProductsDisplayable = true);
    Task<IReadOnlyList<ProductDisplayData>> PopulateIfEmptyAndGetAsync(Func<Task<IEnumerable<ProductDisplayData>>> productFuncAsync, Func<ProductDisplayData, bool> areNewProductsDisplayableFunc);
    void Populate(IEnumerable<ProductDisplayData> products, bool areNewProductsDisplayable = true);
    void Populate(IEnumerable<ProductDisplayData> products, Func<ProductDisplayData, bool> areNewProductsDisplayableFunc);
    bool AddProductDataOrMakeExistingOneDisplayable(ProductDisplayData product, bool makeNewProductFirst = false, bool isNewOrOldProductDisplayable = true);
    bool AddToDisplayableProductIdsIfIdIsntAlreadyAdded(int productId);
    void AddOrUpdateProductData(ProductDisplayData product, bool makeProductFirst = false, bool isNewProductDisplayable = true);
    bool RemoveProductByIndex(int index);
    bool RemoveProductById(int id);
    void RemoveProductIdFromIdsToDisplay(int productId);
    void RemoveAllIdsFromIdsToDisplay();
}