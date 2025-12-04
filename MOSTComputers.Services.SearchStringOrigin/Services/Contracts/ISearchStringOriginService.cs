using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
public interface ISearchStringOriginService
{
    Task<List<SearchStringPartOriginData>?> GetSearchStringPartsAndDataAboutTheirOriginAsync(string? searchString, int? categoryId);
    Task<Dictionary<int, List<SearchStringPartOriginData>>> GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(IEnumerable<Product> products);
}