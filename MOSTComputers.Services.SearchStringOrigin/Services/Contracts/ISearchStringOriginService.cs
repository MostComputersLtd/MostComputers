using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.Services.SearchStringOrigin.Services.Contracts;

public interface ISearchStringOriginService
{
    List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin(Product product);
}